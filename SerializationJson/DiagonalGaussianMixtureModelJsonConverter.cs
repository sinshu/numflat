using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.Clustering;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="DiagonalGaussianMixtureModel" /> instances to and from JSON.
    /// </summary>
    public sealed class DiagonalGaussianMixtureModelJsonConverter : JsonConverter<DiagonalGaussianMixtureModel>
    {
        private const string ComponentsPropertyName = "components";
        private const string WeightPropertyName = "weight";
        private const string GaussianPropertyName = "gaussian";

        /// <inheritdoc />
        public override DiagonalGaussianMixtureModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat diagonal GMM object must be represented as a JSON object.");
            }

            DiagonalGaussianMixtureModel.Component[]? components = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateGmm(components);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat diagonal GMM object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat diagonal GMM object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, ComponentsPropertyName, options))
                {
                    components = ReadComponents(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat diagonal GMM object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DiagonalGaussianMixtureModel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(ComponentsPropertyName);
            writer.WriteStartArray();
            foreach (var component in value.Components)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(WeightPropertyName);
                writer.WriteNumberValue(component.Weight);
                writer.WritePropertyName(GaussianPropertyName);
                WriteGaussian(writer, component.Gaussian, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static DiagonalGaussianMixtureModel.Component[] ReadComponents(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("The 'components' property of a NumFlat diagonal GMM object must be a JSON array.");
            }

            var components = new List<DiagonalGaussianMixtureModel.Component>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return components.ToArray();
                }

                components.Add(ReadComponent(ref reader, options));
            }

            throw new JsonException("The JSON array for NumFlat diagonal GMM components is incomplete.");
        }

        private static DiagonalGaussianMixtureModel.Component ReadComponent(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat diagonal GMM component must be represented as a JSON object.");
            }

            double? weight = null;
            DiagonalGaussian? gaussian = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateComponent(weight, gaussian);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat diagonal GMM component property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat diagonal GMM component is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, WeightPropertyName, options))
                {
                    weight = reader.GetDouble();
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, GaussianPropertyName, options))
                {
                    gaussian = ReadGaussian(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat diagonal GMM component is incomplete.");
        }

        private static DiagonalGaussian ReadGaussian(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return DiagonalGaussianJsonSerialization.Read(ref reader, options, "NumFlat diagonal Gaussian component distribution");
        }

        private static void WriteGaussian(Utf8JsonWriter writer, DiagonalGaussian value, JsonSerializerOptions options)
        {
            DiagonalGaussianJsonSerialization.Write(writer, value, options);
        }

        private static DiagonalGaussianMixtureModel CreateGmm(DiagonalGaussianMixtureModel.Component[]? components)
        {
            if (components == null)
            {
                throw new JsonException("The JSON object for a NumFlat diagonal GMM object must contain a 'components' property.");
            }

            try
            {
                return new DiagonalGaussianMixtureModel(components);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat diagonal GMM object.", ex);
            }
        }

        private static DiagonalGaussianMixtureModel.Component CreateComponent(double? weight, DiagonalGaussian? gaussian)
        {
            if (weight == null)
            {
                throw new JsonException("The JSON object for a NumFlat diagonal GMM component must contain a 'weight' property.");
            }

            if (gaussian == null)
            {
                throw new JsonException("The JSON object for a NumFlat diagonal GMM component must contain a 'gaussian' property.");
            }

            try
            {
                return new DiagonalGaussianMixtureModel.Component(weight.Value, gaussian);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat diagonal GMM component.", ex);
            }
        }

    }
}
