using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class IndependentComponentAnalysisTests
    {
        [Test]
        public void RoundTripIcaKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateIca();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<IndependentComponentAnalysis>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""mean"":[1,2,3],""demixingMatrix"":[[1,2,3],[4,5,6]],""mixingMatrix"":[[7,8],[9,10],[11,12]]}"));
            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
            Assert.That(actual.DemixingMatrix[0, 0], Is.EqualTo(1.0));
            Assert.That(actual.DemixingMatrix[0, 1], Is.EqualTo(2.0));
            Assert.That(actual.DemixingMatrix[0, 2], Is.EqualTo(3.0));
            Assert.That(actual.DemixingMatrix[1, 0], Is.EqualTo(4.0));
            Assert.That(actual.DemixingMatrix[1, 1], Is.EqualTo(5.0));
            Assert.That(actual.DemixingMatrix[1, 2], Is.EqualTo(6.0));
            Assert.That(actual.MixingMatrix[0, 0], Is.EqualTo(7.0));
            Assert.That(actual.MixingMatrix[0, 1], Is.EqualTo(8.0));
            Assert.That(actual.MixingMatrix[1, 0], Is.EqualTo(9.0));
            Assert.That(actual.MixingMatrix[1, 1], Is.EqualTo(10.0));
            Assert.That(actual.MixingMatrix[2, 0], Is.EqualTo(11.0));
            Assert.That(actual.MixingMatrix[2, 1], Is.EqualTo(12.0));
        }

        [Test]
        public void RoundTripIcaKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateIca();
            var x = new Vec<double>(new[] { 5.0, 7.0, 11.0 });
            var y = new Vec<double>(new[] { 13.0, 17.0 });
            var expectedTransformed = source.Transform(x);
            var expectedRestored = source.InverseTransform(y);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<IndependentComponentAnalysis>(json, options)!;
            var actualTransformed = actual.Transform(x);
            var actualRestored = actual.InverseTransform(y);

            Assert.That(actualTransformed.ToArray(), Is.EqualTo(expectedTransformed.ToArray()).Within(1.0e-12));
            Assert.That(actualRestored.ToArray(), Is.EqualTo(expectedRestored.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeIcaHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = @"{""Mean"":[1,2,3],""DemixingMatrix"":[[1,2,3],[4,5,6]],""MixingMatrix"":[[7,8],[9,10],[11,12]]}";

            var actual = JsonSerializer.Deserialize<IndependentComponentAnalysis>(json, options)!;

            Assert.That(actual.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
            Assert.That(actual.DemixingMatrix[1, 2], Is.EqualTo(6.0));
            Assert.That(actual.MixingMatrix[2, 1], Is.EqualTo(12.0));
        }

        [Test]
        public void DeserializeIcaWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""mean"":[1,2,3],""demixingMatrix"":[[1,2],[3,4]],""mixingMatrix"":[[5,6],[7,8],[9,10]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<IndependentComponentAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static IndependentComponentAnalysis CreateIca()
        {
            var mean = new Vec<double>(new[] { 1.0, 2.0, 3.0 });
            Mat<double> demixingMatrix =
            [
                [1.0, 2.0, 3.0],
                [4.0, 5.0, 6.0],
            ];
            Mat<double> mixingMatrix =
            [
                [7.0, 8.0],
                [9.0, 10.0],
                [11.0, 12.0],
            ];

            return new IndependentComponentAnalysis(mean, demixingMatrix, mixingMatrix);
        }
    }
}
