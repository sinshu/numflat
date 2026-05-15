using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            AddConverterIfMissing<VecJsonConverterFactory>(options);
            AddConverterIfMissing<MatJsonConverterFactory>(options);
            AddConverterIfMissing<PrincipalComponentAnalysisJsonConverter>(options);
            AddConverterIfMissing<LinearDiscriminantAnalysisJsonConverter>(options);
            return options;
        }

        private static void AddConverterIfMissing<TConverter>(JsonSerializerOptions options)
            where TConverter : JsonConverter, new()
        {
            foreach (var converter in options.Converters)
            {
                if (converter.GetType() == typeof(TConverter))
                {
                    return;
                }
            }

            options.Converters.Add(new TConverter());
        }
    }
}
