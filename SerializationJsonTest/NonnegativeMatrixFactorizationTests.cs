using System;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class NonnegativeMatrixFactorizationTests
    {
        [Test]
        public void RoundTripNmfKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateNmf();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<NonnegativeMatrixFactorization>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""w"":[[1,2],[3,4],[5,6]],""h"":[[7,8,9],[10,11,12]]}"));
            Assert.That(actual.W[0, 0], Is.EqualTo(1.0));
            Assert.That(actual.W[0, 1], Is.EqualTo(2.0));
            Assert.That(actual.W[1, 0], Is.EqualTo(3.0));
            Assert.That(actual.W[1, 1], Is.EqualTo(4.0));
            Assert.That(actual.W[2, 0], Is.EqualTo(5.0));
            Assert.That(actual.W[2, 1], Is.EqualTo(6.0));
            Assert.That(actual.H[0, 0], Is.EqualTo(7.0));
            Assert.That(actual.H[0, 1], Is.EqualTo(8.0));
            Assert.That(actual.H[0, 2], Is.EqualTo(9.0));
            Assert.That(actual.H[1, 0], Is.EqualTo(10.0));
            Assert.That(actual.H[1, 1], Is.EqualTo(11.0));
            Assert.That(actual.H[1, 2], Is.EqualTo(12.0));
        }

        [Test]
        public void RoundTripNmfKeepsReconstructionBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateNmf();
            var expected = source.W * source.H;

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<NonnegativeMatrixFactorization>(json, options)!;
            var actualReconstruction = actual.W * actual.H;

            Assert.That(actualReconstruction[0, 0], Is.EqualTo(expected[0, 0]).Within(1.0e-12));
            Assert.That(actualReconstruction[0, 1], Is.EqualTo(expected[0, 1]).Within(1.0e-12));
            Assert.That(actualReconstruction[0, 2], Is.EqualTo(expected[0, 2]).Within(1.0e-12));
            Assert.That(actualReconstruction[1, 0], Is.EqualTo(expected[1, 0]).Within(1.0e-12));
            Assert.That(actualReconstruction[1, 1], Is.EqualTo(expected[1, 1]).Within(1.0e-12));
            Assert.That(actualReconstruction[1, 2], Is.EqualTo(expected[1, 2]).Within(1.0e-12));
            Assert.That(actualReconstruction[2, 0], Is.EqualTo(expected[2, 0]).Within(1.0e-12));
            Assert.That(actualReconstruction[2, 1], Is.EqualTo(expected[2, 1]).Within(1.0e-12));
            Assert.That(actualReconstruction[2, 2], Is.EqualTo(expected[2, 2]).Within(1.0e-12));
        }

        [Test]
        public void DeserializeNmfHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = @"{""W"":[[1,2],[3,4],[5,6]],""H"":[[7,8,9],[10,11,12]]}";

            var actual = JsonSerializer.Deserialize<NonnegativeMatrixFactorization>(json, options)!;

            Assert.That(actual.W[2, 1], Is.EqualTo(6.0));
            Assert.That(actual.H[1, 2], Is.EqualTo(12.0));
        }

        [Test]
        public void DeserializeNmfWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""w"":[[1,2],[3,4]],""h"":[[5,6],[7,8],[9,10]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<NonnegativeMatrixFactorization>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static NonnegativeMatrixFactorization CreateNmf()
        {
            Mat<double> w =
            [
                [1.0, 2.0],
                [3.0, 4.0],
                [5.0, 6.0],
            ];
            Mat<double> h =
            [
                [7.0, 8.0, 9.0],
                [10.0, 11.0, 12.0],
            ];

            return new NonnegativeMatrixFactorization(w, h);
        }
    }
}
