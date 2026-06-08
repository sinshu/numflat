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
        private const string KernelMeansPropertyName = "kernelMeans";
        private const string TotalMeanPropertyName = "totalMean";
        private const string ProjectionPropertyName = "projection";

        /// <inheritdoc />
        public override KernelPrincipalComponentAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("A NumFlat kernel PCA object must be represented as a JSON object.");
            }

            Mat<double>? sourceVectors = null;
            Kernel<Vec<double>, Vec<double>>? kernel = null;
            Vec<double>? kernelMeans = null;
            double? totalMean = null;
            Mat<double>? projection = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return CreateKernelPca(sourceVectors, kernel, kernelMeans, totalMean, projection);
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
                    sourceVectors = JsonSerializationHelpers.ReadDoubleMatrix(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, KernelPropertyName, options))
                {
                    kernel = KernelJsonSerialization.Read(ref reader, options, "NumFlat kernel PCA kernel");
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, KernelMeansPropertyName, options))
                {
                    kernelMeans = JsonSerializationHelpers.ReadDoubleVector(ref reader);
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, TotalMeanPropertyName, options))
                {
                    totalMean = reader.GetDouble();
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, ProjectionPropertyName, options))
                {
                    projection = JsonSerializationHelpers.ReadDoubleMatrix(ref reader);
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
            JsonSerializationHelpers.WriteDoubleMatrix(writer, value.SourceVectors);
            writer.WritePropertyName(KernelPropertyName);
            KernelJsonSerialization.Write(writer, value.Kernel, options);
            writer.WritePropertyName(KernelMeansPropertyName);
            JsonSerializationHelpers.WriteDoubleVector(writer, value.KernelMeans);
            writer.WritePropertyName(TotalMeanPropertyName);
            writer.WriteNumberValue(value.TotalMean);
            writer.WritePropertyName(ProjectionPropertyName);
            JsonSerializationHelpers.WriteDoubleMatrix(writer, value.Projection);
            writer.WriteEndObject();
        }

        private static KernelPrincipalComponentAnalysis CreateKernelPca(Mat<double>? sourceVectors, Kernel<Vec<double>, Vec<double>>? kernel, Vec<double>? kernelMeans, double? totalMean, Mat<double>? projection)
        {
            if (sourceVectors == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'sourceVectors' property.");
            }

            if (kernel == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'kernel' property.");
            }

            if (kernelMeans == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'kernelMeans' property.");
            }

            if (totalMean == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'totalMean' property.");
            }

            if (projection == null)
            {
                throw new JsonException("The JSON object for a NumFlat kernel PCA object must contain a 'projection' property.");
            }

            try
            {
                return new KernelPrincipalComponentAnalysis(sourceVectors.Value, kernel, kernelMeans.Value, totalMean.Value, projection.Value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON object cannot be converted to a NumFlat kernel PCA object.", ex);
            }
        }
    }
}
