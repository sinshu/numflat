using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal sealed class VecJsonConverter<T> : JsonConverter<Vec<T>> where T : unmanaged, INumberBase<T>
    {
        public override Vec<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("A NumFlat vector must be represented as a JSON array.");
            }

            var elements = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    if (elements.Count == 0)
                    {
                        throw new JsonException("A NumFlat vector cannot be empty.");
                    }

                    return new Vec<T>(elements.ToArray());
                }

                var element = JsonSerializer.Deserialize<T>(ref reader, options);
                elements.Add(element);
            }

            throw new JsonException("The JSON array for a NumFlat vector is incomplete.");
        }

        public override void Write(Utf8JsonWriter writer, Vec<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var element in value)
            {
                JsonSerializer.Serialize(writer, element, options);
            }
            writer.WriteEndArray();
        }
    }
}
