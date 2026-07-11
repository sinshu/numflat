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
        public void RoundTripKernelPcaStoresTrainingDataAndKernelOnly()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateKernelPca();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""sourceVectors"":[[1,3],[2,4]],""kernel"":{""type"":""gaussian"",""gamma"":0.5}}"));
            Assert.That(actual.SourceVectors[0, 0], Is.EqualTo(1.0));
            Assert.That(actual.SourceVectors[1, 0], Is.EqualTo(2.0));
            Assert.That(actual.SourceVectors[0, 1], Is.EqualTo(3.0));
            Assert.That(actual.SourceVectors[1, 1], Is.EqualTo(4.0));
            Assert.That(actual.Kernel, Is.TypeOf<GaussianKernel>());
            Assert.That(((GaussianKernel)actual.Kernel).Gamma, Is.EqualTo(0.5));
        }

        [Test]
        public void RoundTripKernelPcaKeepsTransformBehaviorByRefitting()
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
        public void DeserializeKernelPcaIgnoresLegacyFittedParametersAndRefits()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""sourceVectors"":[[1,3],[2,4]],""kernel"":{""type"":""gaussian"",""gamma"":0.5},""kernelMeans"":[5,6],""totalMean"":7,""projection"":[[1,0],[0,1]]}";

            var actual = JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)!;
            var expected = CreateKernelPca();
            var x = new Vec<double>(new[] { 5.0, 6.0 });

            Assert.That(actual.Transform(x).ToArray(), Is.EqualTo(expected.Transform(x).ToArray()).Within(1.0e-12));
            Assert.That(actual.KernelMeans.ToArray(), Is.Not.EqualTo(new[] { 5.0, 6.0 }));
            Assert.That(actual.TotalMean, Is.Not.EqualTo(7.0));
        }

        [Test]
        public void DeserializeKernelPcaWithoutKernelThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""sourceVectors"":[[1,3],[2,4]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void DeserializeKernelPcaWithEmptySourceVectorsThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""sourceVectors"":[],""kernel"":{""type"":""linear""}}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<KernelPrincipalComponentAnalysis>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static KernelPrincipalComponentAnalysis CreateKernelPca()
        {
            var xs = new[]
            {
                new Vec<double>(new[] { 1.0, 2.0 }),
                new Vec<double>(new[] { 3.0, 4.0 })
            };

            return new KernelPrincipalComponentAnalysis(xs, Kernel.Gaussian(0.5));
        }
    }
}
