using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class CommonSpatialPatternTests
    {
        [Test]
        public void RoundTripCspKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateCsp();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<CommonSpatialPattern>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""eigenValues"":[3,4],""eigenVectors"":[[0,1],[1,0]]}"));
            Assert.That(actual.EigenValues.ToArray(), Is.EqualTo(new[] { 3.0, 4.0 }));
            Assert.That(actual.EigenVectors[0, 0], Is.EqualTo(0.0));
            Assert.That(actual.EigenVectors[0, 1], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 0], Is.EqualTo(1.0));
            Assert.That(actual.EigenVectors[1, 1], Is.EqualTo(0.0));
        }

        [Test]
        public void RoundTripCspKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateCsp();
            var x = new Vec<double>(new[] { 5.0, 7.0 });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<CommonSpatialPattern>(json, options)!;
            var actualTransformed = actual.Transform(x);

            Assert.That(actualTransformed.ToArray(), Is.EqualTo(expected.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeCspHonorsCaseInsensitivePropertyNames()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            options.PropertyNameCaseInsensitive = true;
            const string json = @"{""EigenValues"":[3,4],""EigenVectors"":[[0,1],[1,0]]}";

            var actual = JsonSerializer.Deserialize<CommonSpatialPattern>(json, options)!;

            Assert.That(actual.EigenValues.ToArray(), Is.EqualTo(new[] { 3.0, 4.0 }));
            Assert.That(actual.EigenVectors[0, 1], Is.EqualTo(1.0));
        }

        [Test]
        public void DeserializeCspWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""eigenValues"":[3],""eigenVectors"":[[1,0],[0,1]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<CommonSpatialPattern>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static CommonSpatialPattern CreateCsp()
        {
            var eigenValues = new Vec<double>(new[] { 3.0, 4.0 });
            var eigenVectors = new Mat<double>(2, 2, 2, new[]
            {
                0.0, 1.0,
                1.0, 0.0
            });

            return new CommonSpatialPattern(eigenValues, eigenVectors);
        }
    }
}
