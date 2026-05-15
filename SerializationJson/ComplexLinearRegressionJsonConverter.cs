using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="ComplexLinearRegression" /> instances to and from JSON.
    /// </summary>
    public sealed class ComplexLinearRegressionJsonConverter : JsonConverter<ComplexLinearRegression>
    {
        private const string CoefficientsPropertyName = "coefficients";
        private const string InterceptPropertyName = "intercept";

        /// <inheritdoc />
        public override ComplexLinearRegression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat complex linear regression object must be represented as a JSON object.");
            }

            Vec<Complex>? coefficients = null;
            Complex? intercept = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateComplexLinearRegression(coefficients, intercept);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat complex linear regression object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat complex linear regression object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, CoefficientsPropertyName, options))
                {
                    coefficients = JsonSerializer.Deserialize<Vec<Complex>>(ref reader, options);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, InterceptPropertyName, options))
                {
                    intercept = JsonSerializer.Deserialize<Complex>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat complex linear regression object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ComplexLinearRegression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(CoefficientsPropertyName);
            JsonSerializer.Serialize(writer, value.Coefficients, options);
            writer.WritePropertyName(InterceptPropertyName);
            JsonSerializer.Serialize(writer, value.Intercept, options);
            writer.WriteEndObject();
        }

        private static ComplexLinearRegression CreateComplexLinearRegression(Vec<Complex>? coefficients, Complex? intercept)
        {
            if (coefficients == null)
            {
                throw new JsonException("The JSON object for a NumFlat complex linear regression object must contain a 'coefficients' property.");
            }

            if (intercept == null)
            {
                throw new JsonException("The JSON object for a NumFlat complex linear regression object must contain an 'intercept' property.");
            }

            try
            {
                return new ComplexLinearRegression(coefficients.Value, intercept.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat complex linear regression object.", ex);
            }
        }
    }
}
