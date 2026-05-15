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
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("A complex value must be represented as a [real, imaginary] array.");
            }

            if (!reader.Read())
            {
                throw new JsonException("The JSON array for a complex value is incomplete.");
            }
            var real = reader.GetDouble();

            if (!reader.Read())
            {
                throw new JsonException("The JSON array for a complex value is incomplete.");
            }
            var imaginary = reader.GetDouble();

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException("A complex value array must contain exactly two numbers.");
            }

            return new Complex(real, imaginary);
        }

        public override void Write(Utf8JsonWriter writer, Complex value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Real);
            writer.WriteNumberValue(value.Imaginary);
            writer.WriteEndArray();
        }
    }
}
