using System.Numerics;
using System.Text.Json.Serialization;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.MultivariateAnalyses;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Provides source-generated JSON metadata for NumFlat types supported by this package.
    /// </summary>
    [JsonSourceGenerationOptions(Converters = new[]
    {
        typeof(ComplexJsonConverter),
        typeof(Int32VectorJsonConverter),
        typeof(SingleVectorJsonConverter),
        typeof(DoubleVectorJsonConverter),
        typeof(ComplexVectorJsonConverter),
        typeof(Int32MatrixJsonConverter),
        typeof(SingleMatrixJsonConverter),
        typeof(DoubleMatrixJsonConverter),
        typeof(ComplexMatrixJsonConverter),
        typeof(GaussianJsonConverter),
        typeof(DiagonalGaussianJsonConverter),
        typeof(KMeansJsonConverter),
        typeof(GaussianMixtureModelJsonConverter),
        typeof(DiagonalGaussianMixtureModelJsonConverter),
        typeof(KernelJsonConverter),
        typeof(PrincipalComponentAnalysisJsonConverter),
        typeof(KernelPrincipalComponentAnalysisJsonConverter),
        typeof(LinearDiscriminantAnalysisJsonConverter),
        typeof(CommonSpatialPatternJsonConverter),
        typeof(IndependentComponentAnalysisJsonConverter),
        typeof(NonnegativeMatrixFactorizationJsonConverter),
        typeof(LinearRegressionJsonConverter),
        typeof(ComplexLinearRegressionJsonConverter),
        typeof(LogisticRegressionJsonConverter)
    })]
    [JsonSerializable(typeof(Complex))]
    [JsonSerializable(typeof(Vec<int>), TypeInfoPropertyName = "VectorInt32")]
    [JsonSerializable(typeof(Vec<float>), TypeInfoPropertyName = "VectorSingle")]
    [JsonSerializable(typeof(Vec<double>), TypeInfoPropertyName = "VectorDouble")]
    [JsonSerializable(typeof(Vec<Complex>), TypeInfoPropertyName = "VectorComplex")]
    [JsonSerializable(typeof(Vec<double>[]), TypeInfoPropertyName = "DoubleVectorArray")]
    [JsonSerializable(typeof(Mat<int>), TypeInfoPropertyName = "MatrixInt32")]
    [JsonSerializable(typeof(Mat<float>), TypeInfoPropertyName = "MatrixSingle")]
    [JsonSerializable(typeof(Mat<double>), TypeInfoPropertyName = "MatrixDouble")]
    [JsonSerializable(typeof(Mat<Complex>), TypeInfoPropertyName = "MatrixComplex")]
    [JsonSerializable(typeof(Gaussian))]
    [JsonSerializable(typeof(DiagonalGaussian))]
    [JsonSerializable(typeof(KMeans))]
    [JsonSerializable(typeof(GaussianMixtureModel), TypeInfoPropertyName = "GaussianMixtureModel")]
    [JsonSerializable(typeof(GaussianMixtureModel.Component), TypeInfoPropertyName = "GaussianMixtureModelComponent")]
    [JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<GaussianMixtureModel.Component>), TypeInfoPropertyName = "GaussianMixtureModelComponentList")]
    [JsonSerializable(typeof(DiagonalGaussianMixtureModel), TypeInfoPropertyName = "DiagonalGaussianMixtureModel")]
    [JsonSerializable(typeof(DiagonalGaussianMixtureModel.Component), TypeInfoPropertyName = "DiagonalGaussianMixtureModelComponent")]
    [JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<DiagonalGaussianMixtureModel.Component>), TypeInfoPropertyName = "DiagonalGaussianMixtureModelComponentList")]
    [JsonSerializable(typeof(Kernel<Vec<double>, Vec<double>>), TypeInfoPropertyName = "DoubleVectorKernel")]
    [JsonSerializable(typeof(PrincipalComponentAnalysis))]
    [JsonSerializable(typeof(KernelPrincipalComponentAnalysis))]
    [JsonSerializable(typeof(LinearDiscriminantAnalysis))]
    [JsonSerializable(typeof(CommonSpatialPattern))]
    [JsonSerializable(typeof(IndependentComponentAnalysis))]
    [JsonSerializable(typeof(NonnegativeMatrixFactorization))]
    [JsonSerializable(typeof(LinearRegression))]
    [JsonSerializable(typeof(ComplexLinearRegression))]
    [JsonSerializable(typeof(LogisticRegression))]
    public sealed partial class NumFlatJsonSerializerContext : JsonSerializerContext
    {
    }
}
