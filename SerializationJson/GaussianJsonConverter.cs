using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NumFlat.Distributions;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts <see cref="Gaussian" /> instances to and from JSON.
    /// </summary>
    public sealed class GaussianJsonConverter : JsonConverter<Gaussian>
    {
        /// <inheritdoc />
        public override Gaussian Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return GaussianJsonSerialization.Read(ref reader, options, "NumFlat Gaussian object");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Gaussian value, JsonSerializerOptions options)
        {
            GaussianJsonSerialization.Write(writer, value, options);
        }
    }
}
