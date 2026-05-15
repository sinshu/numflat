using System;
using System.Text.Json;

namespace NumFlat.Serialization.Json
{
    internal static class EigenTransformJsonConverterCore
    {
        private const string MeanPropertyName = "mean";
        private const string EigenValuesPropertyName = "eigenValues";
        private const string EigenVectorsPropertyName = "eigenVectors";

        public static TTransform Read<TTransform>(
            ref Utf8JsonReader reader,
            JsonSerializerOptions options,
            string modelName,
            Func<Vec<double>, Vec<double>, Mat<double>, TTransform> createTransform)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"A NumFlat {modelName} object must be represented as a JSON object.");
            }

            Vec<double>? mean = null;
            Vec<double>? eigenValues = null;
            Mat<double>? eigenVectors = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateTransform(mean, eigenValues, eigenVectors, modelName, createTransform);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"A NumFlat {modelName} object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException($"The JSON object for a NumFlat {modelName} object is incomplete.");
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

            throw new JsonException($"The JSON object for a NumFlat {modelName} object is incomplete.");
        }

        public static void Write<TTransform>(
            Utf8JsonWriter writer,
            TTransform value,
            JsonSerializerOptions options,
            Func<TTransform, Vec<double>> getMean,
            Func<TTransform, Vec<double>> getEigenValues,
            Func<TTransform, Mat<double>> getEigenVectors)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MeanPropertyName);
            JsonSerializer.Serialize(writer, getMean(value), options);
            writer.WritePropertyName(EigenValuesPropertyName);
            JsonSerializer.Serialize(writer, getEigenValues(value), options);
            writer.WritePropertyName(EigenVectorsPropertyName);
            JsonSerializer.Serialize(writer, getEigenVectors(value), options);
            writer.WriteEndObject();
        }

        private static TTransform CreateTransform<TTransform>(
            Vec<double>? mean,
            Vec<double>? eigenValues,
            Mat<double>? eigenVectors,
            string modelName,
            Func<Vec<double>, Vec<double>, Mat<double>, TTransform> createTransform)
        {
            if (mean == null)
            {
                throw new JsonException($"The JSON object for a NumFlat {modelName} object must contain a 'mean' property.");
            }

            if (eigenValues == null)
            {
                throw new JsonException($"The JSON object for a NumFlat {modelName} object must contain an 'eigenValues' property.");
            }

            if (eigenVectors == null)
            {
                throw new JsonException($"The JSON object for a NumFlat {modelName} object must contain an 'eigenVectors' property.");
            }

            try
            {
                return createTransform(mean.Value, eigenValues.Value, eigenVectors.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException($"The JSON object cannot be converted to a NumFlat {modelName} object.", ex);
            }
        }
    }
}
