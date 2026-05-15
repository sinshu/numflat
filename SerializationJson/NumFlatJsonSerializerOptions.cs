using System.Text.Json;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Provides factory methods for <see cref="JsonSerializerOptions" /> configured for NumFlat types.
    /// </summary>
    public static class NumFlatJsonSerializerOptions
    {
        /// <summary>
        /// Creates serializer options that include converters for <see cref="Vec{T}" /> and <see cref="Mat{T}" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="JsonSerializerOptions" /> instance configured for NumFlat types.
        /// </returns>
        public static JsonSerializerOptions Create()
        {
            return new JsonSerializerOptions().AddNumFlatConverters();
        }

        /// <summary>
        /// Creates serializer options by copying existing options and adding converters for <see cref="Vec{T}" /> and <see cref="Mat{T}" />.
        /// </summary>
        /// <param name="options">
        /// The source options to copy.
        /// </param>
        /// <returns>
        /// A new <see cref="JsonSerializerOptions" /> instance configured for NumFlat types.
        /// </returns>
        public static JsonSerializerOptions Create(JsonSerializerOptions options)
        {
            return new JsonSerializerOptions(options).AddNumFlatConverters();
        }
    }
}
