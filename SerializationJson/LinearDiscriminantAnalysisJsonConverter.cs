using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="LinearDiscriminantAnalysis" /> instances to and from JSON.
    /// </summary>
    public sealed class LinearDiscriminantAnalysisJsonConverter : JsonConverter<LinearDiscriminantAnalysis>
    {
        private const string ModelName = "LDA";

        /// <inheritdoc />
        public override LinearDiscriminantAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EigenTransformJsonConverterCore.Read(ref reader, options, ModelName, CreateTransform);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, LinearDiscriminantAnalysis value, JsonSerializerOptions options)
        {
            EigenTransformJsonConverterCore.Write(writer, value, options, GetMean, GetEigenValues, GetEigenVectors);
        }

        private static LinearDiscriminantAnalysis CreateTransform(Vec<double> mean, Vec<double> eigenValues, Mat<double> eigenVectors)
        {
            return new LinearDiscriminantAnalysis(mean, eigenValues, eigenVectors);
        }

        private static Vec<double> GetMean(LinearDiscriminantAnalysis value)
        {
            return value.Mean;
        }

        private static Vec<double> GetEigenValues(LinearDiscriminantAnalysis value)
        {
            return value.EigenValues;
        }

        private static Mat<double> GetEigenVectors(LinearDiscriminantAnalysis value)
        {
            return value.EigenVectors;
        }
    }
}
