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
    public class SignalProcessingTests
    {
        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void GetFrame_Arg3(int srcLength, int dstLength)
        {
            var source = TestVector.RandomDouble(42, srcLength, 1);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength);
                    var actual = TestVector.RandomDouble(0, dstLength, 1);
                    source.GetFrame(start, actual);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }

        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void GetFrame_Arg2(int srcLength, int dstLength)
        {
            var source = TestVector.RandomDouble(42, srcLength, 1);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength);
                    var actual = source.GetFrame(start, dstLength);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }

        [TestCase(5, 3, 1, 1, 1)]
        [TestCase(5, 3, 3, 2, 4)]
        [TestCase(3, 5, 1, 1, 1)]
        [TestCase(3, 5, 2, 4, 3)]
        public void GetWindowedFrame_Arg3(int srcLength, int dstLength, int srcStride, int winStride, int dstStride)
        {
            var source = TestVector.RandomDouble(42, srcLength, srcStride);
            var window = TestVector.RandomDouble(57, dstLength, winStride);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).PointwiseMul(window);

                    var actual = TestVector.RandomDouble(0, dstLength, dstStride);
                    source.GetWindowedFrame(start, window, actual);

                    NumAssert.AreSame(expected, actual, 0);
                    TestVector.FailIfOutOfRangeWrite(actual);
                }
            }
        }

        [TestCase(5, 3, 1, 1)]
        [TestCase(5, 3, 3, 2)]
        [TestCase(3, 5, 1, 1)]
        [TestCase(3, 5, 2, 4)]
        public void GetWindowedFrame_Arg2(int srcLength, int dstLength, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, srcLength, srcStride);
            var window = TestVector.RandomDouble(57, dstLength, winStride);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).PointwiseMul(window);
                    var actual = source.GetWindowedFrame(start, window);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }
    }
}
