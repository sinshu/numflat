using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="PrincipalComponentAnalysis" /> instances to and from JSON.
    /// </summary>
    public sealed class PrincipalComponentAnalysisJsonConverter : JsonConverter<PrincipalComponentAnalysis>
    {
        private const string MeanPropertyName = "mean";
        private const string EigenValuesPropertyName = "eigenValues";
        private const string EigenVectorsPropertyName = "eigenVectors";

        /// <inheritdoc />
        public override PrincipalComponentAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat PCA object must be represented as a JSON object.");
            }

            Vec<double>? mean = null;
            Vec<double>? eigenValues = null;
            Mat<double>? eigenVectors = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreatePca(mean, eigenValues, eigenVectors);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat PCA object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat PCA object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, MeanPropertyName, options))
                {
                    mean = JsonSerializer.Deserialize<Vec<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, EigenValuesPropertyName, options))
                {
                    eigenValues = JsonSerializer.Deserialize<Vec<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, EigenVectorsPropertyName, options))
                {
                    eigenVectors = JsonSerializer.Deserialize<Mat<double>>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat PCA object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PrincipalComponentAnalysis value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MeanPropertyName);
            JsonSerializer.Serialize(writer, value.Mean, options);
            writer.WritePropertyName(EigenValuesPropertyName);
            JsonSerializer.Serialize(writer, value.EigenValues, options);
            writer.WritePropertyName(EigenVectorsPropertyName);
            JsonSerializer.Serialize(writer, value.EigenVectors, options);
            writer.WriteEndObject();
        }

        private static PrincipalComponentAnalysis CreatePca(Vec<double>? mean, Vec<double>? eigenValues, Mat<double>? eigenVectors)
        {
            if (mean == null)
            {
                throw new JsonException("The JSON object for a NumFlat PCA object must contain a 'mean' property.");
            }

            if (eigenValues == null)
            {
                throw new JsonException("The JSON object for a NumFlat PCA object must contain an 'eigenValues' property.");
            }

            if (eigenVectors == null)
            {
                throw new JsonException("The JSON object for a NumFlat PCA object must contain an 'eigenVectors' property.");
            }

            try
            {
                return new PrincipalComponentAnalysis(mean.Value, eigenValues.Value, eigenVectors.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat PCA object.", ex);
            }
        }
    }
}
