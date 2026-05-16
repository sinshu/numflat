using System;
using System.Text.Json;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    internal static class DiagonalGaussianJsonSerialization
    {
        private const string MeanPropertyName = "mean";
        private const string VariancePropertyName = "variance";

        public static DiagonalGaussian Read(ref Utf8JsonReader reader, JsonSerializerOptions options, string objectDescription)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"A {objectDescription} must be represented as a JSON object.");
            }

            Vec<double>? mean = null;
            Vec<double>? variance = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return Create(mean, variance, objectDescription);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"A {objectDescription} property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException($"The JSON object for a {objectDescription} is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, MeanPropertyName, options))
                {
                    mean = JsonSerializer.Deserialize<Vec<double>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, VariancePropertyName, options))
                {
                    variance = JsonSerializer.Deserialize<Vec<double>>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException($"The JSON object for a {objectDescription} is incomplete.");
        }

        public static void Write(Utf8JsonWriter writer, DiagonalGaussian value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MeanPropertyName);
            JsonSerializer.Serialize(writer, value.Mean, options);
            writer.WritePropertyName(VariancePropertyName);
            JsonSerializer.Serialize(writer, value.Variance, options);
            writer.WriteEndObject();
        }

        private static DiagonalGaussian Create(Vec<double>? mean, Vec<double>? variance, string objectDescription)
        {
            if (mean == null)
            {
                throw new JsonException($"The JSON object for a {objectDescription} must contain a 'mean' property.");
            }

            if (variance == null)
            {
                throw new JsonException($"The JSON object for a {objectDescription} must contain a 'variance' property.");
            }

            try
            {
                return new DiagonalGaussian(mean.Value, variance.Value);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FittingFailureException)
            {
                throw new JsonException($"The JSON object cannot be converted to a {objectDescription}.", ex);
            }
        }
    }
}
