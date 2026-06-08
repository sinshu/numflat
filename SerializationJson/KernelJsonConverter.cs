using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    /// <summary>
    /// Converts built-in <see cref="Kernel{T, U}" /> instances for double vectors to and from JSON.
    /// </summary>
    public sealed class KernelJsonConverter : JsonConverter<Kernel<Vec<double>, Vec<double>>>
    {
        /// <inheritdoc />
        public override Kernel<Vec<double>, Vec<double>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return KernelJsonSerialization.Read(ref reader, options, "NumFlat kernel object");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Kernel<Vec<double>, Vec<double>> value, JsonSerializerOptions options)
        {
            KernelJsonSerialization.Write(writer, value, options);
        }
    }
}
