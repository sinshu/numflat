using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using MathNet.Numerics.IntegralTransforms;
using NumFlat;
using NumFlat.FourierTransform;

namespace NumFlatTest
{
    public class FourierTransformTest
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
    }
}
