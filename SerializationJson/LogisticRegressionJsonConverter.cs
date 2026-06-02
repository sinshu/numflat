using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="LogisticRegression" /> instances to and from JSON.
    /// </summary>
    public sealed class LogisticRegressionJsonConverter : JsonConverter<LogisticRegression>
    {
        private const string CoefficientsPropertyName = "coefficients";
        private const string InterceptPropertyName = "intercept";

        /// <inheritdoc />
        public override LogisticRegression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat logistic regression object must be represented as a JSON object.");
            }

            Vec<double>? coefficients = null;
            double? intercept = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateLogisticRegression(coefficients, intercept);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat logistic regression object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat logistic regression object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, CoefficientsPropertyName, options))
                {
                    coefficients = JsonSerializationHelpers.ReadDoubleVector(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, InterceptPropertyName, options))
                {
                    intercept = reader.GetDouble();
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat logistic regression object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, LogisticRegression value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(CoefficientsPropertyName);
            JsonSerializationHelpers.WriteDoubleVector(writer, value.Coefficients);
            writer.WritePropertyName(InterceptPropertyName);
            writer.WriteNumberValue(value.Intercept);
            writer.WriteEndObject();
        }

        private static LogisticRegression CreateLogisticRegression(Vec<double>? coefficients, double? intercept)
        {
            if (coefficients == null)
            {
                throw new JsonException("The JSON object for a NumFlat logistic regression object must contain a 'coefficients' property.");
            }

            if (intercept == null)
            {
                throw new JsonException("The JSON object for a NumFlat logistic regression object must contain an 'intercept' property.");
            }

            try
            {
                return new LogisticRegression(coefficients.Value, intercept.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat logistic regression object.", ex);
            }
        }
    }
}
