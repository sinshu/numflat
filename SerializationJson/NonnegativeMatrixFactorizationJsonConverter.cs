using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="NonnegativeMatrixFactorization" /> instances to and from JSON.
    /// </summary>
    public sealed class NonnegativeMatrixFactorizationJsonConverter : JsonConverter<NonnegativeMatrixFactorization>
    {
        private const string WPropertyName = "w";
        private const string HPropertyName = "h";

        /// <inheritdoc />
        public override NonnegativeMatrixFactorization Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat NMF object must be represented as a JSON object.");
            }

            Mat<double>? w = null;
            Mat<double>? h = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateNmf(w, h);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat NMF object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat NMF object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, WPropertyName, options))
                {
                    w = JsonSerializer.Deserialize<Mat<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, HPropertyName, options))
                {
                    h = JsonSerializer.Deserialize<Mat<double>>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat NMF object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, NonnegativeMatrixFactorization value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(WPropertyName);
            JsonSerializer.Serialize(writer, value.W, options);
            writer.WritePropertyName(HPropertyName);
            JsonSerializer.Serialize(writer, value.H, options);
            writer.WriteEndObject();
        }

        private static NonnegativeMatrixFactorization CreateNmf(Mat<double>? w, Mat<double>? h)
        {
            if (w == null)
            {
                throw new JsonException("The JSON object for a NumFlat NMF object must contain a 'w' property.");
            }

            if (h == null)
            {
                throw new JsonException("The JSON object for a NumFlat NMF object must contain an 'h' property.");
            }

            try
            {
                return new NonnegativeMatrixFactorization(w.Value, h.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat NMF object.", ex);
            }
        }
    }
}
