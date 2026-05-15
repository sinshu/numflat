using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class LinearDiscriminantAnalysisTests
    {
        [Test]
        public void RoundTripLdaKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLda();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LinearDiscriminantAnalysis>(json, options)!;

            Assert.That(json, Is.EqualTo("{\"mean\":[1,2],\"eigenValues\":[3,4],\"eigenVectors\":[[0,1],[1,0]]}"));
            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.EigenValues.ToArray(), Is.EqualTo(new[] { 3.0, 4.0 }));
            Assert.That(actual.EigenVectors[0, 0], Is.EqualTo(0.0));
            Assert.That(actual.EigenVectors[0, 1], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 0], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 1], Is.EqualTo(0.0));
        }

        [Test]
        public void RoundTripLdaKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateLda();
            var x = new Vec<double>(new[] { 5.0, 7.0 });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<LinearDiscriminantAnalysis>(json, options)!;
            var actualTransformed = actual.Transform(x);

            Assert.That(actualTransformed.ToArray(), Is.EqualTo(expected.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeLdaWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = "{\"mean\":[1,2],\"eigenValues\":[3],\"eigenVectors\":[[1,0],[0,1]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<LinearDiscriminantAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static LinearDiscriminantAnalysis CreateLda()
        {
            var mean = new Vec<double>(new[] { 1.0, 2.0 });
            var eigenValues = new Vec<double>(new[] { 3.0, 4.0 });
            var eigenVectors = new Mat<double>(2, 2, 2, new[]
            {
                0.0, 1.0,
                1.0, 0.0
            });

            return new LinearDiscriminantAnalysis(mean, eigenValues, eigenVectors);
        }
    }
}
