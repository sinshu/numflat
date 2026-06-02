using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal abstract class VectorJsonConverter<T> : JsonConverter<Vec<T>> where T : unmanaged, INumberBase<T>
    {
        public override Vec<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadVector(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, Vec<T> value, JsonSerializerOptions options)
        {
            WriteVector(writer, value);
        }

        public Vec<T> ReadVector(ref Utf8JsonReader reader)
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
                        return default;
                    }

                    return new Vec<T>(elements.ToArray());
                }

                elements.Add(ReadElement(ref reader));
            }

            throw new JsonException("The JSON array for a NumFlat vector is incomplete.");
        }

        public void WriteVector(Utf8JsonWriter writer, Vec<T> value)
        {
            writer.WriteStartArray();
            foreach (var element in value)
            {
                WriteElement(writer, element);
            }
            writer.WriteEndArray();
        }

        protected abstract T ReadElement(ref Utf8JsonReader reader);

        protected abstract void WriteElement(Utf8JsonWriter writer, T value);
    }

    internal sealed class Int32VectorJsonConverter : VectorJsonConverter<int>
    {
        protected override int ReadElement(ref Utf8JsonReader reader) => reader.GetInt32();

        protected override void WriteElement(Utf8JsonWriter writer, int value) => writer.WriteNumberValue(value);
    }

    internal sealed class SingleVectorJsonConverter : VectorJsonConverter<float>
    {
        protected override float ReadElement(ref Utf8JsonReader reader) => reader.GetSingle();

        protected override void WriteElement(Utf8JsonWriter writer, float value) => writer.WriteNumberValue(value);
    }

    internal sealed class DoubleVectorJsonConverter : VectorJsonConverter<double>
    {
        protected override double ReadElement(ref Utf8JsonReader reader) => reader.GetDouble();

        protected override void WriteElement(Utf8JsonWriter writer, double value) => writer.WriteNumberValue(value);
    }

    internal sealed class ComplexVectorJsonConverter : VectorJsonConverter<Complex>
    {
        protected override Complex ReadElement(ref Utf8JsonReader reader) => JsonSerializationHelpers.ReadComplex(ref reader);

        protected override void WriteElement(Utf8JsonWriter writer, Complex value) => JsonSerializationHelpers.WriteComplex(writer, value);
    }
}
