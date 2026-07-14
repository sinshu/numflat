using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="KernelPrincipalComponentAnalysis" /> instances to and from JSON.
    /// </summary>
    public sealed class KernelPrincipalComponentAnalysisJsonConverter : JsonConverter<KernelPrincipalComponentAnalysis>
    {
        private const string SourceVectorsPropertyName = "sourceVectors";
        private const string KernelPropertyName = "kernel";

        /// <inheritdoc />
        public override KernelPrincipalComponentAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat kernel PCA object must be represented as a JSON object.");
            }

            Vec<double>[]? sourceVectors = null;
            Kernel<Vec<double>, Vec<double>>? kernel = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateKernelPca(sourceVectors, kernel);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("A NumFlat kernel PCA object property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException("The JSON object for a NumFlat kernel PCA object is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, SourceVectorsPropertyName, options))
                {
                    sourceVectors = JsonSerializationHelpers.ReadDoubleVectorArray(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, KernelPropertyName, options))
                {
                    kernel = KernelJsonSerialization.Read(ref reader, options, "NumFlat kernel PCA kernel");
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException("The JSON object for a NumFlat kernel PCA object is incomplete.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, KernelPrincipalComponentAnalysis value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(SourceVectorsPropertyName);
            JsonSerializationHelpers.WriteDoubleVectorArray(writer, value.SourceVectors);
            writer.WritePropertyName(KernelPropertyName);
            KernelJsonSerialization.Write(writer, value.Kernel, options);
            writer.WriteEndObject();
        }

        private static KernelPrincipalComponentAnalysis CreateKernelPca(Vec<double>[]? sourceVectors, Kernel<Vec<double>, Vec<double>>? kernel)
        {
            if (sourceVectors == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'sourceVectors' property.");
            }

            if (kernel == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'kernel' property.");
            }

            try
            {
                return new KernelPrincipalComponentAnalysis(sourceVectors, kernel);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat kernel PCA object.", ex);
            }
        }
    }
}
