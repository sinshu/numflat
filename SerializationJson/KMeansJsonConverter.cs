using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.Clustering;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="KMeans" /> instances to and from JSON.
    /// </summary>
    public sealed class KMeansJsonConverter : JsonConverter<KMeans>
    {
        private const string CentroidsPropertyName = "centroids";

        /// <inheritdoc />
        public override KMeans Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat k-means object must be represented as a JSON object.");
            }

            Vec<double>[]? centroids = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateKMeans(centroids);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat k-means object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat k-means object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, CentroidsPropertyName, options))
                {
                    centroids = JsonSerializationHelpers.ReadDoubleVectorArray(ref reader);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat k-means object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, KMeans value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(CentroidsPropertyName);
            JsonSerializationHelpers.WriteDoubleVectorArray(writer, value.Centroids);
            writer.WriteEndObject();
        }

        private static KMeans CreateKMeans(Vec<double>[]? centroids)
        {
            if (centroids == null)
            {
                throw new JsonException("The JSON object for a NumFlat k-means object must contain a 'centroids' property.");
            }

            try
            {
                return new KMeans(centroids);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat k-means object.", ex);
            }
        }
    }
}
