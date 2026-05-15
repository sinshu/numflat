using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumFlat.Serialization.Json
{
    internal static class JsonSerializationHelpers
    {
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

        public static JsonConverter CreateGenericConverter(Type genericConverterTypeDefinition, Type elementType)
        {
            var converterType = genericConverterTypeDefinition.MakeGenericType(elementType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
