using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="DiagonalGaussian" /> instances to and from JSON.
    /// </summary>
    public sealed class DiagonalGaussianJsonConverter : JsonConverter<DiagonalGaussian>
    {
        /// <inheritdoc />
        public override DiagonalGaussian Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DiagonalGaussianJsonSerialization.Read(ref reader, options, "NumFlat diagonal Gaussian object");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DiagonalGaussian value, JsonSerializerOptions options)
        {
            DiagonalGaussianJsonSerialization.Write(writer, value, options);
        }
    }
}
