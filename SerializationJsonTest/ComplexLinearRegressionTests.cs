using System;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class ComplexLinearRegressionTests
    {
        [Test]
        public void RoundTripComplexLinearRegressionKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateComplexLinearRegression();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<ComplexLinearRegression>(json, options)!;

            Assert.That(json, Is.EqualTo("{\"coefficients\":[[1,2],[3,4]],\"intercept\":[5,6]}"));
            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { new Complex(1.0, 2.0), new Complex(3.0, 4.0) }));
            Assert.That(actual.Intercept, Is.EqualTo(new Complex(5.0, 6.0)));
        }

        [Test]
        public void RoundTripComplexLinearRegressionKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateComplexLinearRegression();
            var x = new Vec<Complex>(new[] { new Complex(7.0, 8.0), new Complex(9.0, 10.0) });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<ComplexLinearRegression>(json, options)!;
            var actualTransformed = actual.Transform(x);

            Assert.That(actualTransformed.Real, Is.EqualTo(expected.Real).Within(1.0e-12));
            Assert.That(actualTransformed.Imaginary, Is.EqualTo(expected.Imaginary).Within(1.0e-12));
        }

        [Test]
        public void DeserializeComplexLinearRegressionHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = "{\"Coefficients\":[[1,2],[3,4]],\"Intercept\":[5,6]}";

            var actual = JsonSerializer.Deserialize<ComplexLinearRegression>(json, options)!;

            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { new Complex(1.0, 2.0), new Complex(3.0, 4.0) }));
            Assert.That(actual.Intercept, Is.EqualTo(new Complex(5.0, 6.0)));
        }

        [Test]
        public void DeserializeComplexLinearRegressionWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = "{\"coefficients\":[],\"intercept\":[5,6]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<ComplexLinearRegression>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static ComplexLinearRegression CreateComplexLinearRegression()
        {
            var coefficients = new Vec<Complex>(new[] { new Complex(1.0, 2.0), new Complex(3.0, 4.0) });

            return new ComplexLinearRegression(coefficients, new Complex(5.0, 6.0));
        }
    }
}
