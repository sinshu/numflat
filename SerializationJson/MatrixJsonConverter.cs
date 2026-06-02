using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal abstract class MatrixJsonConverter<T> : JsonConverter<Mat<T>> where T : unmanaged, INumberBase<T>
    {
        public override Mat<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadMatrix(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, Mat<T> value, JsonSerializerOptions options)
        {
            WriteMatrix(writer, value);
        }

        public Mat<T> ReadMatrix(ref Utf8JsonReader reader)
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

                var row = ReadRow(ref reader);
                rows.Add(row);
            }

            throw new JsonException("The JSON array for a NumFlat matrix is incomplete.");
        }

        public void WriteMatrix(Utf8JsonWriter writer, Mat<T> value)
        {
            writer.WriteStartArray();
            for (var row = 0; row < value.RowCount; row++)
            {
                writer.WriteStartArray();
                for (var col = 0; col < value.ColCount; col++)
                {
                    WriteElement(writer, value[row, col]);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        protected abstract T ReadElement(ref Utf8JsonReader reader);

        protected abstract void WriteElement(Utf8JsonWriter writer, T value);

        private Vec<T> ReadRow(ref Utf8JsonReader reader)
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

                elements.Add(ReadElement(ref reader));
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

    internal sealed class Int32MatrixJsonConverter : MatrixJsonConverter<int>
    {
        protected override int ReadElement(ref Utf8JsonReader reader) => reader.GetInt32();

        protected override void WriteElement(Utf8JsonWriter writer, int value) => writer.WriteNumberValue(value);
    }

    internal sealed class SingleMatrixJsonConverter : MatrixJsonConverter<float>
    {
        protected override float ReadElement(ref Utf8JsonReader reader) => reader.GetSingle();

        protected override void WriteElement(Utf8JsonWriter writer, float value) => writer.WriteNumberValue(value);
    }

    internal sealed class DoubleMatrixJsonConverter : MatrixJsonConverter<double>
    {
        protected override double ReadElement(ref Utf8JsonReader reader) => reader.GetDouble();

        protected override void WriteElement(Utf8JsonWriter writer, double value) => writer.WriteNumberValue(value);
    }

    internal sealed class ComplexMatrixJsonConverter : MatrixJsonConverter<Complex>
    {
        protected override Complex ReadElement(ref Utf8JsonReader reader) => JsonSerializationHelpers.ReadComplex(ref reader);

        protected override void WriteElement(Utf8JsonWriter writer, Complex value) => JsonSerializationHelpers.WriteComplex(writer, value);
    }
}
