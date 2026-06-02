using System;
using System.Text.Json;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    internal static class GaussianJsonSerialization
    {
        private const string MeanPropertyName = "mean";
        private const string CovariancePropertyName = "covariance";

        public static Gaussian Read(ref Utf8JsonReader reader, JsonSerializerOptions options, string objectDescription)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"A {objectDescription} must be represented as a JSON object.");
            }

            Vec<double>? mean = null;
            Mat<double>? covariance = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return Create(mean, covariance, objectDescription);
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
                    mean = JsonSerializationHelpers.ReadDoubleVector(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, CovariancePropertyName, options))
                {
                    covariance = JsonSerializationHelpers.ReadDoubleMatrix(ref reader);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException($"The JSON object for a {objectDescription} is incomplete.");
        }

        public static void Write(Utf8JsonWriter writer, Gaussian value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MeanPropertyName);
            JsonSerializationHelpers.WriteDoubleVector(writer, value.Mean);
            writer.WritePropertyName(CovariancePropertyName);
            JsonSerializationHelpers.WriteDoubleMatrix(writer, value.Covariance);
            writer.WriteEndObject();
        }

        private static Gaussian Create(Vec<double>? mean, Mat<double>? covariance, string objectDescription)
        {
            if (mean == null)
            {
                throw new JsonException($"The JSON object for a {objectDescription} must contain a 'mean' property.");
            }

            if (covariance == null)
            {
                throw new JsonException($"The JSON object for a {objectDescription} must contain a 'covariance' property.");
            }

            try
            {
                return new Gaussian(mean.Value, covariance.Value);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FittingFailureException || ex is MatrixFactorizationException)
            {
                throw new JsonException($"The JSON object cannot be converted to a {objectDescription}.", ex);
            }
        }
    }
}
