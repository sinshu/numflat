using System;
using System.Text.Json;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Provides extension methods for configuring <see cref="JsonSerializerOptions" /> to handle NumFlat types.
    /// </summary>
    public static class NumFlatJsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Adds converters for NumFlat types to the specified options.
        /// </summary>
        /// <param name="options">
        /// The serializer options to configure.
        /// </param>
        /// <returns>
        /// The same <see cref="JsonSerializerOptions" /> instance so calls can be chained.
        /// </returns>
        public static JsonSerializerOptions AddNumFlatConverters(this JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            JsonSerializationHelpers.AddConverterIfMissing<ComplexJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<VecJsonConverterFactory>(options);
            JsonSerializationHelpers.AddConverterIfMissing<MatJsonConverterFactory>(options);
            JsonSerializationHelpers.AddConverterIfMissing<GaussianJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<DiagonalGaussianJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<KMeansJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<GaussianMixtureModelJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<DiagonalGaussianMixtureModelJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<PrincipalComponentAnalysisJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<LinearDiscriminantAnalysisJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<CommonSpatialPatternJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<IndependentComponentAnalysisJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<NonnegativeMatrixFactorizationJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<LinearRegressionJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<ComplexLinearRegressionJsonConverter>(options);
            JsonSerializationHelpers.AddConverterIfMissing<LogisticRegressionJsonConverter>(options);
            return options;
        }
    }
}
