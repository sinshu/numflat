using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="CommonSpatialPattern" /> instances to and from JSON.
    /// </summary>
    public sealed class CommonSpatialPatternJsonConverter : JsonConverter<CommonSpatialPattern>
    {
        private const string EigenValuesPropertyName = "eigenValues";
        private const string EigenVectorsPropertyName = "eigenVectors";

        /// <inheritdoc />
        public override CommonSpatialPattern Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat CSP object must be represented as a JSON object.");
            }

            Vec<double>? eigenValues = null;
            Mat<double>? eigenVectors = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateCsp(eigenValues, eigenVectors);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat CSP object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat CSP object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, EigenValuesPropertyName, options))
                {
                    eigenValues = JsonSerializationHelpers.ReadDoubleVector(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, EigenVectorsPropertyName, options))
                {
                    eigenVectors = JsonSerializationHelpers.ReadDoubleMatrix(ref reader);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat CSP object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CommonSpatialPattern value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(EigenValuesPropertyName);
            JsonSerializationHelpers.WriteDoubleVector(writer, value.EigenValues);
            writer.WritePropertyName(EigenVectorsPropertyName);
            JsonSerializationHelpers.WriteDoubleMatrix(writer, value.EigenVectors);
            writer.WriteEndObject();
        }

        private static CommonSpatialPattern CreateCsp(Vec<double>? eigenValues, Mat<double>? eigenVectors)
        {
            if (eigenValues == null)
            {
                throw new JsonException("The JSON object for a NumFlat CSP object must contain an 'eigenValues' property.");
            }

            if (eigenVectors == null)
            {
                throw new JsonException("The JSON object for a NumFlat CSP object must contain an 'eigenVectors' property.");
            }

            try
            {
                return new CommonSpatialPattern(eigenValues.Value, eigenVectors.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat CSP object.", ex);
            }
        }
    }
}
