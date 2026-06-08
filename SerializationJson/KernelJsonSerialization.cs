using System;
using System.Text.Json;

namespace NumFlat.Serialization.Json
{
    internal static class KernelJsonSerialization
    {
        private const string TypePropertyName = "type";
        private const string DegreePropertyName = "degree";
        private const string ConstantPropertyName = "constant";
        private const string GammaPropertyName = "gamma";
        private const string LinearTypeName = "linear";
        private const string PolynomialTypeName = "polynomial";
        private const string GaussianTypeName = "gaussian";

        public static Kernel<Vec<double>, Vec<double>> Read(ref Utf8JsonReader reader, JsonSerializerOptions options, string objectDescription)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"A {objectDescription} must be represented as a JSON object.");
            }

            string? type = null;
            int? degree = null;
            double? constant = null;
            double? gamma = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return Create(type, degree, constant, gamma, objectDescription);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"A {objectDescription} property name is expected.");
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException($"The JSON object for a {objectDescription} is incomplete.");
                }

                if (JsonSerializationHelpers.PropertyNameEquals(propertyName, TypePropertyName, options))
                {
                    type = reader.GetString();
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, DegreePropertyName, options))
                {
                    degree = reader.GetInt32();
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, ConstantPropertyName, options))
                {
                    constant = reader.GetDouble();
                }
                else if (JsonSerializationHelpers.PropertyNameEquals(propertyName, GammaPropertyName, options))
                {
                    gamma = reader.GetDouble();
                }
                else
                {
                    reader.Skip();
                }
            }

            throw new JsonException($"The JSON object for a {objectDescription} is incomplete.");
        }

        public static void Write(Utf8JsonWriter writer, Kernel<Vec<double>, Vec<double>> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case LinearKernel:
                    writer.WriteString(TypePropertyName, LinearTypeName);
                    break;

                case PolynomialKernel polynomial:
                    writer.WriteString(TypePropertyName, PolynomialTypeName);
                    writer.WriteNumber(DegreePropertyName, polynomial.Degree);
                    writer.WriteNumber(ConstantPropertyName, polynomial.Constant);
                    break;

                case GaussianKernel gaussian:
                    writer.WriteString(TypePropertyName, GaussianTypeName);
                    writer.WriteNumber(GammaPropertyName, gaussian.Gamma);
                    break;

                default:
                    throw new NotSupportedException($"The kernel type '{value.GetType()}' is not supported by NumFlat JSON serialization.");
            }

            writer.WriteEndObject();
        }

        private static Kernel<Vec<double>, Vec<double>> Create(string? type, int? degree, double? constant, double? gamma, string objectDescription)
        {
            if (type == null)
            {
                throw new JsonException($"The JSON object for a {objectDescription} must contain a 'type' property.");
            }

            try
            {
                switch (type)
                {
                    case LinearTypeName:
                        return new LinearKernel();

                    case PolynomialTypeName:
                        if (degree == null)
                        {
                            throw new JsonException($"The JSON object for a {objectDescription} with type 'polynomial' must contain a 'degree' property.");
                        }

                        if (constant == null)
                        {
                            throw new JsonException($"The JSON object for a {objectDescription} with type 'polynomial' must contain a 'constant' property.");
                        }

                        return new PolynomialKernel(degree.Value, constant.Value);

                    case GaussianTypeName:
                        if (gamma == null)
                        {
                            throw new JsonException($"The JSON object for a {objectDescription} with type 'gaussian' must contain a 'gamma' property.");
                        }

                        return new GaussianKernel(gamma.Value);

                    default:
                        throw new JsonException($"The JSON object for a {objectDescription} contains an unsupported kernel type '{type}'.");
                }
            }
            catch (ArgumentException ex)
            {
                throw new JsonException($"The JSON object cannot be converted to a {objectDescription}.", ex);
            }
        }
    }
}
