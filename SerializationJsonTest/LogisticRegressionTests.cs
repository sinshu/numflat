using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class LogisticRegressionTests
    {
        [Test]
        public void RoundTripLogisticRegressionKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLogisticRegression();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LogisticRegression>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""coefficients"":[1,2],""intercept"":3}"));
            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Intercept, Is.EqualTo(3.0));
        }

        [Test]
        public void RoundTripLogisticRegressionKeepsTransformAndPredictionBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLogisticRegression();
            var x = new Vec<double>(new[] { 5.0, 7.0 });
            var expected = source.Transform(x);
            var expectedPrediction = source.Predict(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LogisticRegression>(json, options)!;
            var actualTransformed = actual.Transform(x);
            var actualPrediction = actual.Predict(x);

            Assert.That(actualTransformed, Is.EqualTo(expected).Within(1.0e-12));
            Assert.That(actualPrediction, Is.EqualTo(expectedPrediction));
        }

        [Test]
        public void DeserializeLogisticRegressionHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = @"{""Coefficients"":[1,2],""Intercept"":3}";

            var actual = JsonSerializer.Deserialize<LogisticRegression>(json, options)!;

            Assert.That(actual.Coefficients.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Intercept, Is.EqualTo(3.0));
        }

        [Test]
        public void DeserializeLogisticRegressionWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""coefficients"":[],""intercept"":3}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<LogisticRegression>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static LogisticRegression CreateLogisticRegression()
        {
            var coefficients = new Vec<double>(new[] { 1.0, 2.0 });

            return new LogisticRegression(coefficients, 3.0);
        }
    }
}
