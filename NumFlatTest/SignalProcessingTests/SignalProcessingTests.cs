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
    }
}
