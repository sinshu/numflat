using System;
using System.Text.Json;
using NumFlat;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class KernelTests
    {
        [Test]
        public void RoundTripLinearKernelKeepsBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            Kernel<Vec<double>, Vec<double>> source = Kernel.Linear();

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Kernel<Vec<double>, Vec<double>>>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""type"":""linear""}"));
            Assert.That(actual, Is.TypeOf<LinearKernel>());
            AssertKernelBehavior(actual, source);
        }

        [Test]
        public void RoundTripPolynomialKernelKeepsParametersAndBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            Kernel<Vec<double>, Vec<double>> source = Kernel.Polynomial(3, 1.5);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Kernel<Vec<double>, Vec<double>>>(json, options)!;
            var polynomial = (PolynomialKernel)actual;

            Assert.That(json, Is.EqualTo(@"{""type"":""polynomial"",""degree"":3,""constant"":1.5}"));
            Assert.That(polynomial.Degree, Is.EqualTo(3));
            Assert.That(polynomial.Constant, Is.EqualTo(1.5));
            AssertKernelBehavior(actual, source);
        }

        [Test]
        public void RoundTripGaussianKernelKeepsParametersAndBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            Kernel<Vec<double>, Vec<double>> source = Kernel.Gaussian(0.5);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Kernel<Vec<double>, Vec<double>>>(json, options)!;
            var gaussian = (GaussianKernel)actual;

            Assert.That(json, Is.EqualTo(@"{""type"":""gaussian"",""gamma"":0.5}"));
            Assert.That(gaussian.Gamma, Is.EqualTo(0.5));
            AssertKernelBehavior(actual, source);
        }

        [Test]
        public void DeserializeKernelWithUnknownTypeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""type"":""sigmoid""}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<Kernel<Vec<double>, Vec<double>>>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void DeserializePolynomialKernelWithoutRequiredPropertyThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""type"":""polynomial"",""degree"":3}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<Kernel<Vec<double>, Vec<double>>>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static void AssertKernelBehavior(Kernel<Vec<double>, Vec<double>> actual, Kernel<Vec<double>, Vec<double>> expected)
        {
            var x = new Vec<double>(new[] { 1.0, 2.0 });
            var y = new Vec<double>(new[] { 3.0, 4.0 });

            Assert.That(actual.Invoke(x, y), Is.EqualTo(expected.Invoke(x, y)).Within(1.0e-12));
        }
    }
}
