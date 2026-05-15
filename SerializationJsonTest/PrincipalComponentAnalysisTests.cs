using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class PrincipalComponentAnalysisTests
    {
        [Test]
        public void RoundTripPcaKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreatePca();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<PrincipalComponentAnalysis>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""mean"":[1,2],""eigenValues"":[3,4],""eigenVectors"":[[0,1],[1,0]]}"));
            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.EigenValues.ToArray(), Is.EqualTo(new[] { 3.0, 4.0 }));
            Assert.That(actual.EigenVectors[0, 0], Is.EqualTo(0.0));
            Assert.That(actual.EigenVectors[0, 1], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 0], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 1], Is.EqualTo(0.0));
        }

        [Test]
        public void RoundTripPcaKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreatePca();
            var x = new Vec<double>(new[] { 5.0, 7.0 });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<PrincipalComponentAnalysis>(json, options)!;
            var actualTransformed = actual.Transform(x);
            var restored = actual.InverseTransform(actualTransformed);

            Assert.That(actualTransformed.ToArray(), Is.EqualTo(expected.ToArray()).Within(1.0e-12));
            Assert.That(restored.ToArray(), Is.EqualTo(x.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializePcaWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2],""eigenValues"":[3],""eigenVectors"":[[1,0],[0,1]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<PrincipalComponentAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static PrincipalComponentAnalysis CreatePca()
        {
            var mean = new Vec<double>(new[] { 1.0, 2.0 });
            var eigenValues = new Vec<double>(new[] { 3.0, 4.0 });
            var eigenVectors = new Mat<double>(2, 2, 2, new[]
            {
                0.0, 1.0,
                1.0, 0.0
            });

            return new PrincipalComponentAnalysis(mean, eigenValues, eigenVectors);
        }
    }
}
