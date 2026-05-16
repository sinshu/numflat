using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.Clustering;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="GaussianMixtureModel" /> instances to and from JSON.
    /// </summary>
    public sealed class GaussianMixtureModelJsonConverter : JsonConverter<GaussianMixtureModel>
    {
        private const string ComponentsPropertyName = "components";
        private const string WeightPropertyName = "weight";
        private const string GaussianPropertyName = "gaussian";

        /// <inheritdoc />
        public override GaussianMixtureModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat GMM object must be represented as a JSON object.");
            }

            GaussianMixtureModel.Component[]? components = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateGmm(components);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat GMM object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat GMM object is incomplete.");
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

            throw new JsonException("The JSON object for a NumFlat GMM object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, GaussianMixtureModel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(ComponentsPropertyName);
            writer.WriteStartArray();
            foreach (var component in value.Components)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(WeightPropertyName);
                JsonSerializer.Serialize(writer, component.Weight, options);
                writer.WritePropertyName(GaussianPropertyName);
                WriteGaussian(writer, component.Gaussian, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static GaussianMixtureModel.Component[] ReadComponents(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("The 'components' property of a NumFlat GMM object must be a JSON array.");
            }

            var components = new List<GaussianMixtureModel.Component>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return components.ToArray();
                }

                components.Add(ReadComponent(ref reader, options));
            }

            throw new JsonException("The JSON array for NumFlat GMM components is incomplete.");
        }

        private static GaussianMixtureModel.Component ReadComponent(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat GMM component must be represented as a JSON object.");
            }

            double? weight = null;
            Gaussian? gaussian = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateComponent(weight, gaussian);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat GMM component property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat GMM component is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, WeightPropertyName, options))
                {
                    weight = JsonSerializer.Deserialize<double>(ref reader, options);
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

            throw new JsonException("The JSON object for a NumFlat GMM component is incomplete.");
        }

        private static Gaussian ReadGaussian(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return GaussianJsonSerialization.Read(ref reader, options, "NumFlat Gaussian component distribution");
        }

        private static void WriteGaussian(Utf8JsonWriter writer, Gaussian value, JsonSerializerOptions options)
        {
            GaussianJsonSerialization.Write(writer, value, options);
        }

        private static GaussianMixtureModel CreateGmm(GaussianMixtureModel.Component[]? components)
        {
            if (components == null)
            {
                throw new JsonException("The JSON object for a NumFlat GMM object must contain a 'components' property.");
            }

            try
            {
                return new GaussianMixtureModel(components);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat GMM object.", ex);
            }
        }

        private static GaussianMixtureModel.Component CreateComponent(double? weight, Gaussian? gaussian)
        {
            if (weight == null)
            {
                throw new JsonException("The JSON object for a NumFlat GMM component must contain a 'weight' property.");
            }

            if (gaussian == null)
            {
                throw new JsonException("The JSON object for a NumFlat GMM component must contain a 'gaussian' property.");
            }

            try
            {
                return new GaussianMixtureModel.Component(weight.Value, gaussian);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat GMM component.", ex);
            }
        }

    }
}
