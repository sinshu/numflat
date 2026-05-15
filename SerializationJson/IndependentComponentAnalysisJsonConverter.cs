using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="IndependentComponentAnalysis" /> instances to and from JSON.
    /// </summary>
    public sealed class IndependentComponentAnalysisJsonConverter : JsonConverter<IndependentComponentAnalysis>
    {
        private const string MeanPropertyName = "mean";
        private const string DemixingMatrixPropertyName = "demixingMatrix";
        private const string MixingMatrixPropertyName = "mixingMatrix";

        /// <inheritdoc />
        public override IndependentComponentAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat ICA object must be represented as a JSON object.");
            }

            Vec<double>? mean = null;
            Mat<double>? demixingMatrix = null;
            Mat<double>? mixingMatrix = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateIca(mean, demixingMatrix, mixingMatrix);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat ICA object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat ICA object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, MeanPropertyName, options))
                {
                    mean = JsonSerializer.Deserialize<Vec<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, DemixingMatrixPropertyName, options))
                {
                    demixingMatrix = JsonSerializer.Deserialize<Mat<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, MixingMatrixPropertyName, options))
                {
                    mixingMatrix = JsonSerializer.Deserialize<Mat<double>>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat ICA object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IndependentComponentAnalysis value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MeanPropertyName);
            JsonSerializer.Serialize(writer, value.Mean, options);
            writer.WritePropertyName(DemixingMatrixPropertyName);
            JsonSerializer.Serialize(writer, value.DemixingMatrix, options);
            writer.WritePropertyName(MixingMatrixPropertyName);
            JsonSerializer.Serialize(writer, value.MixingMatrix, options);
            writer.WriteEndObject();
        }

        private static IndependentComponentAnalysis CreateIca(Vec<double>? mean, Mat<double>? demixingMatrix, Mat<double>? mixingMatrix)
        {
            if (mean == null)
            {
                throw new JsonException("The JSON object for a NumFlat ICA object must contain a 'mean' property.");
            }

            if (demixingMatrix == null)
            {
                throw new JsonException("The JSON object for a NumFlat ICA object must contain a 'demixingMatrix' property.");
            }

            if (mixingMatrix == null)
            {
                throw new JsonException("The JSON object for a NumFlat ICA object must contain a 'mixingMatrix' property.");
            }

            try
            {
                return new IndependentComponentAnalysis(mean.Value, demixingMatrix.Value, mixingMatrix.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat ICA object.", ex);
            }
        }
    }
}
