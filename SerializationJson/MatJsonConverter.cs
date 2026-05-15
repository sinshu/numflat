using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal sealed class MatJsonConverter<T> : JsonConverter<Mat<T>> where T : unmanaged, INumberBase<T>
    {
        public override Mat<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("A NumFlat matrix must be represented as a JSON array of row arrays.");
            }

            var rows = new List<Vec<T>>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return CreateMatrix(rows);
                }

                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException("Each NumFlat matrix row must be represented as a JSON array.");
                }

                var row = ReadRow(ref reader, options);
                rows.Add(row);
            }

            throw new JsonException("The JSON array for a NumFlat matrix is incomplete.");
        }

        public override void Write(Utf8JsonWriter writer, Mat<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for (var row = 0; row < value.RowCount; row++)
            {
                writer.WriteStartArray();
                for (var col = 0; col < value.ColCount; col++)
                {
                    JsonSerializer.Serialize(writer, value[row, col], options);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        private static Vec<T> ReadRow(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var elements = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    if (elements.Count == 0)
                    {
                        throw new JsonException("A NumFlat matrix row cannot be empty.");
                    }

                    return new Vec<T>(elements.ToArray());
                }

                var element = JsonSerializer.Deserialize<T>(ref reader, options);
                elements.Add(element);
            }

            throw new JsonException("The JSON array for a NumFlat matrix row is incomplete.");
        }

        private static Mat<T> CreateMatrix(List<Vec<T>> rows)
        {
            if (rows.Count == 0)
            {
                return default;
            }

            try
            {
                return MatrixBuilder.Create<T>(rows.ToArray());
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("The JSON array cannot be converted to a NumFlat matrix.", ex);
            }
        }
    }
}
