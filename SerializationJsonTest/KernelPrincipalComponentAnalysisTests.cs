using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.MultivariateAnalyses;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class KernelPrincipalComponentAnalysisTests
    {
        [Test]
        public void RoundTripKernelPcaKeepsFittedParameters()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateKernelPca();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""sourceVectors"":[[1,3],[2,4]],""kernel"":{""type"":""gaussian"",""gamma"":0.5},""kernelMeans"":[5,6],""totalMean"":7,""projection"":[[1,0],[0,1]]}"));
            Assert.That(actual.SourceVectors[0, 0], Is.EqualTo(1.0));
            Assert.That(actual.SourceVectors[1, 0], Is.EqualTo(2.0));
            Assert.That(actual.SourceVectors[0, 1], Is.EqualTo(3.0));
            Assert.That(actual.SourceVectors[1, 1], Is.EqualTo(4.0));
            Assert.That(actual.Kernel, Is.TypeOf<GaussianKernel>());
            Assert.That(((GaussianKernel)actual.Kernel).Gamma, Is.EqualTo(0.5));
            Assert.That(actual.KernelMeans.ToArray(), Is.EqualTo(new[] { 5.0, 6.0 }));
            Assert.That(actual.TotalMean, Is.EqualTo(7.0));
            Assert.That(actual.Projection[0, 0], Is.EqualTo(1.0));
            Assert.That(actual.Projection[0, 1], Is.EqualTo(0.0));
            Assert.That(actual.Projection[1, 0], Is.EqualTo(0.0));
            Assert.That(actual.Projection[1, 1], Is.EqualTo(1.0));
        }

        [Test]
        public void RoundTripKernelPcaKeepsTransformBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateKernelPca();
            var x = new Vec<double>(new[] { 5.0, 6.0 });
            var expected = source.Transform(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)!;
            var actualTransformed = actual.Transform(x);

            Assert.That(actualTransformed.ToArray(), Is.EqualTo(expected.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeKernelPcaWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""sourceVectors"":[[1,3],[2,4]],""kernel"":{""type"":""linear""},""kernelMeans"":[5],""totalMean"":7,""projection"":[[1,0],[0,1]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static KernelPrincipalComponentAnalysis CreateKernelPca()
        {
            var sourceVectors = new Mat<double>(2, 2, 2, new[]
            {
                1.0, 2.0,
                3.0, 4.0
            });
            var kernelMeans = new Vec<double>(new[] { 5.0, 6.0 });
            var projection = new Mat<double>(2, 2, 2, new[]
            {
                1.0, 0.0,
                0.0, 1.0
            });

            return new KernelPrincipalComponentAnalysis(sourceVectors, Kernel.Gaussian(0.5), kernelMeans, 7.0, projection);
        }
    }
}
