using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using MathNet.Numerics.IntegralTransforms;
using NumFlat;
using NumFlat.SignalProcessing;

namespace NumFlatTest.SignalProcessingTests
{
    public class FourierTransformTests
    {
        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 3, 3)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 6, 5)]
        [TestCase(8, 1, 1)]
        [TestCase(8, 9, 10)]
        [TestCase(16, 1, 1)]
        [TestCase(16, 17, 17)]
        [TestCase(32, 1, 1)]
        [TestCase(32, 34, 33)]
        public void Forward_Arg2(int length, int srcStride, int dstStride)
        {
            var src = TestVector.RandomComplex(42, length, srcStride);
            var actual = TestVector.RandomComplex(0, length, dstStride);

            using (src.EnsureUnchanged())
            {
                FourierTransform.Fft(src, actual);
            }

            var expected = src.ToArray();
            Fourier.Forward(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 3)]
        [TestCase(2, 1, 1)]
        [TestCase(2, 3, 3)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 6, 5)]
        [TestCase(8, 1, 1)]
        [TestCase(8, 9, 10)]
        [TestCase(16, 1, 1)]
        [TestCase(16, 17, 17)]
        [TestCase(32, 1, 1)]
        [TestCase(32, 34, 33)]
        public void Inverse_Arg2(int length, int srcStride, int dstStride)
        {
            var src = TestVector.RandomComplex(42, length, srcStride);
            var actual = TestVector.RandomComplex(0, length, dstStride);

            using (src.EnsureUnchanged())
            {
                FourierTransform.Ifft(src, actual);
            }

            var expected = src.ToArray();
            Fourier.Inverse(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 35)]
        public void Forward_Arg1(int length, int srcStride)
        {
            var src = TestVector.RandomComplex(42, length, srcStride);

            Vec<Complex> actual;
            using (src.EnsureUnchanged())
            {
                actual = src.Fft();
            }

            var expected = src.ToArray();
            Fourier.Forward(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 35)]
        public void Inverse_Arg2(int length, int srcStride)
        {
            var src = TestVector.RandomComplex(42, length, srcStride);

            Vec<Complex> actual;
            using (src.EnsureUnchanged())
            {
                actual = src.Ifft();
            }

            var expected = src.ToArray();
            Fourier.Inverse(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 35)]
        public void Forward_Inplace(int length, int srcStride)
        {
            var actual = TestVector.RandomComplex(42, length, srcStride);

            var expected = actual.ToArray();
            Fourier.Forward(expected, FourierOptions.AsymmetricScaling);

            actual.FftInplace();

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 35)]
        public void Inverse_Inplace(int length, int srcStride)
        {
            var actual = TestVector.RandomComplex(42, length, srcStride);

            var expected = actual.ToArray();
            Fourier.Inverse(expected, FourierOptions.AsymmetricScaling);

            actual.IfftInplace();

            NumAssert.AreSame(expected.ToVector(), actual, 1.0E-12);
        }

        [TestCase(2, 1, 1)]
        [TestCase(2, 3, 3)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 6, 5)]
        [TestCase(8, 1, 1)]
        [TestCase(8, 9, 10)]
        [TestCase(16, 1, 1)]
        [TestCase(16, 17, 17)]
        [TestCase(32, 1, 1)]
        [TestCase(32, 34, 33)]
        public void ForwardReal_Arg2(int length, int srcStride, int dstStride)
        {
            var src = TestVector.RandomDouble(42, length, srcStride);
            var actual = TestVector.RandomComplex(0, length / 2 + 1, dstStride);

            using (src.EnsureUnchanged())
            {
                FourierTransform.Rfft(src, actual);
            }

            var expected = src.Select(x => (Complex)x).ToArray();
            Fourier.Forward(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.Take(actual.Count).ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(2, 1, 1)]
        [TestCase(2, 3, 3)]
        [TestCase(4, 1, 1)]
        [TestCase(4, 6, 5)]
        [TestCase(8, 1, 1)]
        [TestCase(8, 9, 10)]
        [TestCase(16, 1, 1)]
        [TestCase(16, 17, 17)]
        [TestCase(32, 1, 1)]
        [TestCase(32, 34, 33)]
        public void InverseReal_Arg2(int length, int srcStride, int dstStride)
        {
            var src = TestVector.RandomComplex(42, length / 2 + 1, srcStride);
            src[0] = src[0].Real;
            src[length / 2] = src[length / 2].Real;
            var actual = TestVector.RandomDouble(0, length, dstStride);

            using (src.EnsureUnchanged())
            {
                FourierTransform.Irfft(src, actual);
            }

            var expected = new Complex[length];
            src.CopyTo(expected.AsSpan(0, src.Count));
            for (var w = 1; w < length / 2; w++)
            {
                expected[length - w] = expected[w].Conjugate();
            }
            Fourier.Inverse(expected, FourierOptions.AsymmetricScaling);
            foreach (var value in expected.Select(x => x.Imaginary))
            {
                Assert.That(value, Is.EqualTo(0.0).Within(1.0E-12));
            }

            NumAssert.AreSame(expected.Select(x => x.Real).ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 34)]
        public void ForwardReal_Arg1(int length, int srcStride)
        {
            var src = TestVector.RandomDouble(42, length, srcStride);

            Vec<Complex> actual;
            using (src.EnsureUnchanged())
            {
                actual = FourierTransform.Rfft(src);
            }

            var expected = src.Select(x => (Complex)x).ToArray();
            Fourier.Forward(expected, FourierOptions.AsymmetricScaling);

            NumAssert.AreSame(expected.Take(actual.Count).ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }

        [TestCase(2, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 1)]
        [TestCase(4, 6)]
        [TestCase(8, 1)]
        [TestCase(8, 9)]
        [TestCase(16, 1)]
        [TestCase(16, 17)]
        [TestCase(32, 1)]
        [TestCase(32, 34)]
        public void InverseReal_Arg1(int length, int srcStride)
        {
            var src = TestVector.RandomComplex(42, length / 2 + 1, srcStride);
            src[0] = src[0].Real;
            src[length / 2] = src[length / 2].Real;

            Vec<double> actual;
            using (src.EnsureUnchanged())
            {
                actual = FourierTransform.Irfft(src);
            }

            var expected = new Complex[length];
            src.CopyTo(expected.AsSpan(0, src.Count));
            for (var w = 1; w < length / 2; w++)
            {
                expected[length - w] = expected[w].Conjugate();
            }
            Fourier.Inverse(expected, FourierOptions.AsymmetricScaling);
            foreach (var value in expected.Select(x => x.Imaginary))
            {
                Assert.That(value, Is.EqualTo(0.0).Within(1.0E-12));
            }

            NumAssert.AreSame(expected.Select(x => x.Real).ToVector(), actual, 1.0E-12);

            TestVector.FailIfOutOfRangeWrite(actual);
        }
    }
}
