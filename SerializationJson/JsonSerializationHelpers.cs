using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal static class JsonSerializationHelpers
    {
        private static readonly DoubleVectorJsonConverter DoubleVectorConverter = new DoubleVectorJsonConverter();
        private static readonly DoubleMatrixJsonConverter DoubleMatrixConverter = new DoubleMatrixJsonConverter();
        private static readonly ComplexVectorJsonConverter ComplexVectorConverter = new ComplexVectorJsonConverter();
        private static readonly ComplexMatrixJsonConverter ComplexMatrixConverter = new ComplexMatrixJsonConverter();

        public static bool PropertyNameEquals(string? value, string expected, JsonSerializerOptions options)
        {
            var comparison = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return string.Equals(value, expected, comparison);
        }

        public static void AddConverterIfMissing<TConverter>(JsonSerializerOptions options)
            where TConverter : JsonConverter, new()
        {
            foreach (var converter in options.Converters)
            {
                if (converter.GetType() == typeof(TConverter))
                {
                    return;
                }
            }

            options.Converters.Add(new TConverter());
        }

        public static Complex ReadComplex(ref Utf8JsonReader reader)
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

        public static void WriteComplex(Utf8JsonWriter writer, Complex value)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Real);
            writer.WriteNumberValue(value.Imaginary);
            writer.WriteEndArray();
        }

        public static Vec<double> ReadDoubleVector(ref Utf8JsonReader reader)
        {
            return DoubleVectorConverter.ReadVector(ref reader);
        }

        public static void WriteDoubleVector(Utf8JsonWriter writer, Vec<double> value)
        {
            DoubleVectorConverter.WriteVector(writer, value);
        }

        public static Mat<double> ReadDoubleMatrix(ref Utf8JsonReader reader)
        {
            return DoubleMatrixConverter.ReadMatrix(ref reader);
        }

        public static void WriteDoubleMatrix(Utf8JsonWriter writer, Mat<double> value)
        {
            DoubleMatrixConverter.WriteMatrix(writer, value);
        }

        public static Vec<Complex> ReadComplexVector(ref Utf8JsonReader reader)
        {
            return ComplexVectorConverter.ReadVector(ref reader);
        }

        public static void WriteComplexVector(Utf8JsonWriter writer, Vec<Complex> value)
        {
            ComplexVectorConverter.WriteVector(writer, value);
        }

        public static Mat<Complex> ReadComplexMatrix(ref Utf8JsonReader reader)
        {
            return ComplexMatrixConverter.ReadMatrix(ref reader);
        }

        public static void WriteComplexMatrix(Utf8JsonWriter writer, Mat<Complex> value)
        {
            ComplexMatrixConverter.WriteMatrix(writer, value);
        }

        public static Vec<double>[] ReadDoubleVectorArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("A NumFlat vector array must be represented as a JSON array.");
            }

            var values = new List<Vec<double>>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return values.ToArray();
                }

                values.Add(ReadDoubleVector(ref reader));
            }

            throw new JsonException("The JSON array for NumFlat vectors is incomplete.");
        }

        public static void WriteDoubleVectorArray(Utf8JsonWriter writer, IReadOnlyList<Vec<double>> values)
        {
            writer.WriteStartArray();
            foreach (var value in values)
            {
                WriteDoubleVector(writer, value);
            }
            writer.WriteEndArray();
        }
    }
}
