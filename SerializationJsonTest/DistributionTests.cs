using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.Distributions;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class DistributionTests
    {
        [Test]
        public void RoundTripGaussianKeepsParametersAndProbabilityBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateGaussian();
            var x = new Vec<double>(new[] { 1.2, 2.1 });
            var expectedProbability = source.Pdf(x);
            var expectedLogProbability = source.LogPdf(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Gaussian>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""mean"":[1,2],""covariance"":[[2,0.1],[0.1,3]]}"));
            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Covariance[0, 0], Is.EqualTo(2.0));
            Assert.That(actual.Covariance[0, 1], Is.EqualTo(0.1));
            Assert.That(actual.Covariance[1, 0], Is.EqualTo(0.1));
            Assert.That(actual.Covariance[1, 1], Is.EqualTo(3.0));
            Assert.That(actual.Pdf(x), Is.EqualTo(expectedProbability).Within(1.0e-12));
            Assert.That(actual.LogPdf(x), Is.EqualTo(expectedLogProbability).Within(1.0e-12));
        }

        [Test]
        public void DeserializeGaussianWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2],""covariance"":[[1,0,0],[0,1,0],[0,0,1]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<Gaussian>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void DeserializeGaussianWithoutRequiredPropertyThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<Gaussian>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void RoundTripDiagonalGaussianKeepsParametersAndProbabilityBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateDiagonalGaussian();
            var x = new Vec<double>(new[] { 1.2, 2.1 });
            var expectedProbability = source.Pdf(x);
            var expectedLogProbability = source.LogPdf(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<DiagonalGaussian>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""mean"":[1,2],""variance"":[2,3]}"));
            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Variance.ToArray(), Is.EqualTo(new[] { 2.0, 3.0 }));
            Assert.That(actual.Pdf(x), Is.EqualTo(expectedProbability).Within(1.0e-12));
            Assert.That(actual.LogPdf(x), Is.EqualTo(expectedLogProbability).Within(1.0e-12));
        }

        [Test]
        public void DeserializeDiagonalGaussianWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2],""variance"":[1]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<DiagonalGaussian>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void DeserializeDiagonalGaussianWithoutRequiredPropertyThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<DiagonalGaussian>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static Gaussian CreateGaussian()
        {
            return new Gaussian(
                new Vec<double>(new[] { 1.0, 2.0 }),
                new Mat<double>(2, 2, 2, new[]
                {
                    2.0, 0.1,
                    0.1, 3.0
                }));
        }

        private static DiagonalGaussian CreateDiagonalGaussian()
        {
            return new DiagonalGaussian(
                new Vec<double>(new[] { 1.0, 2.0 }),
                new Vec<double>(new[] { 2.0, 3.0 }));
        }
    }
}
