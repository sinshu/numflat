using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="PrincipalComponentAnalysis" /> instances to and from JSON.
    /// </summary>
    public sealed class PrincipalComponentAnalysisJsonConverter : JsonConverter<PrincipalComponentAnalysis>
    {
        private const string ModelName = "PCA";

        /// <inheritdoc />
        public override PrincipalComponentAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EigenTransformJsonConverterCore.Read(ref reader, options, ModelName, CreateTransform);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PrincipalComponentAnalysis value, JsonSerializerOptions options)
        {
            EigenTransformJsonConverterCore.Write(writer, value, options, GetMean, GetEigenValues, GetEigenVectors);
        }

        private static PrincipalComponentAnalysis CreateTransform(Vec<double> mean, Vec<double> eigenValues, Mat<double> eigenVectors)
        {
            return new PrincipalComponentAnalysis(mean, eigenValues, eigenVectors);
        }

        private static Vec<double> GetMean(PrincipalComponentAnalysis value)
        {
            return value.Mean;
        }

        private static Vec<double> GetEigenValues(PrincipalComponentAnalysis value)
        {
            return value.EigenValues;
        }

        private static Mat<double> GetEigenVectors(PrincipalComponentAnalysis value)
        {
            return value.EigenVectors;
        }
    }
}
