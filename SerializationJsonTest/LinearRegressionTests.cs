using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class LinearRegressionTests
    {
        [Test]
        public void RoundTripLinearRegressionKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLinearRegression();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LinearRegression>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""coefficients"":[1,2],""intercept"":3}"));
            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Intercept, Is.EqualTo(3.0));
        }

        [Test]
        public void RoundTripLinearRegressionKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLinearRegression();
            var x = new Vec<double>(new[] { 5.0, 7.0 });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LinearRegression>(json, options)!;
            var actualTransformed = actual.Transform(x);

            Assert.That(actualTransformed, Is.EqualTo(expected).Within(1.0e-12));
        }

        [Test]
        public void DeserializeLinearRegressionHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = @"{""Coefficients"":[1,2],""Intercept"":3}";

            var actual = JsonSerializer.Deserialize<LinearRegression>(json, options)!;

            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Intercept, Is.EqualTo(3.0));
        }

        [Test]
        public void DeserializeLinearRegressionWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""coefficients"":[],""intercept"":3}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<LinearRegression>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static LinearRegression CreateLinearRegression()
        {
            var coefficients = new Vec<double>(new[] { 1.0, 2.0 });

            return new LinearRegression(coefficients, 3.0);
        }
    }
}
