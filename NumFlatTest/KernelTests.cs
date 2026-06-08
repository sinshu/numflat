using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class KernelTests
    {
        [Test]
        public void Polynomial_ReturnsReadableParameters()
        {
            var kernel = Kernel.Polynomial(3, 2.0);

            Assert.That(kernel.Degree, Is.EqualTo(3));
            Assert.That(kernel.Constant, Is.EqualTo(2.0));
        }

        [Test]
        public void Gaussian_ReturnsReadableParameters()
        {
            var kernel = Kernel.Gaussian(0.5);

            Assert.That(kernel.Gamma, Is.EqualTo(0.5));
        }

        [Test]
        public void Invoke_ComputesKernelValues()
        {
            var x = new[] { 1.0, 2.0 }.ToVector();
            var y = new[] { 3.0, 4.0 }.ToVector();

            Assert.That(Kernel.Linear().Invoke(x, y), Is.EqualTo(11.0).Within(1.0E-12));
            Assert.That(Kernel.Polynomial(2, 1.0).Invoke(x, y), Is.EqualTo(144.0).Within(1.0E-12));
            Assert.That(Kernel.Gaussian(0.5).Invoke(x, y), Is.EqualTo(System.Math.Exp(-4.0)).Within(1.0E-12));
        }
    }
}
