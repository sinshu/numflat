using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal sealed class ComplexJsonConverter : JsonConverter<Complex>
    {
        public override Complex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializationHelpers.ReadComplex(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, Complex value, JsonSerializerOptions options)
        {
            JsonSerializationHelpers.WriteComplex(writer, value);
        }
    }
}
