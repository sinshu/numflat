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

        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void GetFrameAsComplex_Arg3(int srcLength, int dstLength)
        {
            var source = TestVector.RandomDouble(42, srcLength, 1);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).Map(x => (Complex)x);
                    var actual = TestVector.RandomComplex(0, dstLength, 1);
                    source.GetFrameAsComplex(start, actual);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }

        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void GetFrameAsComplex_Arg2(int srcLength, int dstLength)
        {
            var source = TestVector.RandomDouble(42, srcLength, 1);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).Map(x => (Complex)x);
                    var actual = source.GetFrameAsComplex(start, dstLength);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }

        [TestCase(5, 3, 1, 1, 1)]
        [TestCase(5, 3, 3, 2, 4)]
        [TestCase(3, 5, 1, 1, 1)]
        [TestCase(3, 5, 2, 4, 3)]
        public void GetWindowedFrameAsComplex_Arg3(int srcLength, int dstLength, int srcStride, int winStride, int dstStride)
        {
            var source = TestVector.RandomDouble(42, srcLength, srcStride);
            var window = TestVector.RandomDouble(57, dstLength, winStride);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).PointwiseMul(window).Map(x => (Complex)x);

                    var actual = TestVector.RandomComplex(0, dstLength, dstStride);
                    source.GetWindowedFrameAsComplex(start, window, actual);

                    NumAssert.AreSame(expected, actual, 0);
                    TestVector.FailIfOutOfRangeWrite(actual);
                }
            }
        }

        [TestCase(5, 3, 1, 1)]
        [TestCase(5, 3, 3, 2)]
        [TestCase(3, 5, 1, 1)]
        [TestCase(3, 5, 2, 4)]
        public void GetWindowedFrameAsComplex_Arg2(int srcLength, int dstLength, int srcStride, int winStride)
        {
            var source = TestVector.RandomDouble(42, srcLength, srcStride);
            var window = TestVector.RandomDouble(57, dstLength, winStride);

            var pad = 10;
            var paddedSource = new double[pad].Concat(source).Concat(new double[pad]).ToVector();

            using (source.EnsureUnchanged())
            {
                for (var start = -dstLength - 3; start <= srcLength + 3; start++)
                {
                    var expected = paddedSource.Subvector(pad + start, dstLength).PointwiseMul(window).Map(x => (Complex)x);
                    var actual = source.GetWindowedFrameAsComplex(start, window);
                    NumAssert.AreSame(expected, actual, 0);
                }
            }
        }

        [TestCase(5, 3)]
        [TestCase(3, 5)]
        public void OverlapAdd(int trgLength, int frmLength)
        {
            for (var start = -frmLength - 3; start <= trgLength + 3; start++)
            {
                var target = TestVector.RandomDouble(42, trgLength, 1);
                var frame = TestVector.RandomDouble(57, frmLength, 1);

                var pad = 10;
                var padded = new double[pad].Concat(target).Concat(new double[pad]).ToVector();
                padded.Subvector(pad + start, frmLength).AddInplace(frame);
                var expected = padded.Skip(pad).SkipLast(pad).ToVector();

                target.OverlapAdd(start, frame);
                NumAssert.AreSame(expected, target, 1.0E-12);
            }
        }

        [TestCase(5, 3, 1, 1)]
        [TestCase(5, 3, 3, 2)]
        [TestCase(3, 5, 1, 1)]
        [TestCase(3, 5, 2, 4)]
        public void OverlapAdd_Complex(int trgLength, int frmLength, int trgStride, int frmStride)
        {
            for (var start = -frmLength - 3; start <= trgLength + 3; start++)
            {
                var target = TestVector.RandomDouble(42, trgLength, trgStride);
                var frame = TestVector.RandomComplex(57, frmLength, frmStride);

                var pad = 10;
                var padded = new double[pad].Concat(target).Concat(new double[pad]).ToVector();
                padded.Subvector(pad + start, frmLength).AddInplace(frame.Map(x => x.Real));
                var expected = padded.Skip(pad).SkipLast(pad).ToVector();

                target.OverlapAdd(start, frame);

                NumAssert.AreSame(expected, target, 1.0E-12);

                TestVector.FailIfOutOfRangeWrite(target);
            }
        }

        [TestCase(5, 3, 1, 1, 1)]
        [TestCase(5, 3, 3, 2, 4)]
        [TestCase(3, 5, 1, 1, 1)]
        [TestCase(3, 5, 2, 4, 3)]
        public void WindowedOverlapAdd(int trgLength, int frmLength, int trgStride, int winStride, int frmStride)
        {
            for (var start = -frmLength - 3; start <= trgLength + 3; start++)
            {
                var target = TestVector.RandomDouble(42, trgLength, trgStride);
                var window = TestVector.RandomDouble(57, frmLength, winStride);
                var frame = TestVector.RandomDouble(66, frmLength, frmStride);

                var pad = 10;
                var padded = new double[pad].Concat(target).Concat(new double[pad]).ToVector();
                var add = frame.PointwiseMul(window);
                padded.Subvector(pad + start, frmLength).AddInplace(add);
                var expected = padded.Skip(pad).SkipLast(pad).ToVector();

                target.WindowedOverlapAdd(start, window, frame);

                NumAssert.AreSame(expected, target, 1.0E-12);

                TestVector.FailIfOutOfRangeWrite(target);
            }
        }

        [TestCase(5, 3, 1, 1, 1)]
        [TestCase(5, 3, 3, 2, 4)]
        [TestCase(3, 5, 1, 1, 1)]
        [TestCase(3, 5, 2, 4, 3)]
        public void WindowedOverlapAdd_Complex(int trgLength, int frmLength, int trgStride, int winStride, int frmStride)
        {
            for (var start = -frmLength - 3; start <= trgLength + 3; start++)
            {
                var target = TestVector.RandomDouble(42, trgLength, trgStride);
                var window = TestVector.RandomDouble(57, frmLength, winStride);
                var frame = TestVector.RandomComplex(66, frmLength, frmStride);

                var pad = 10;
                var padded = new double[pad].Concat(target).Concat(new double[pad]).ToVector();
                var add = frame.Map(x => x.Real).PointwiseMul(window);
                padded.Subvector(pad + start, frmLength).AddInplace(add);
                var expected = padded.Skip(pad).SkipLast(pad).ToVector();

                target.WindowedOverlapAdd(start, window, frame);

                NumAssert.AreSame(expected, target, 1.0E-12);

                TestVector.FailIfOutOfRangeWrite(target);
            }
        }
    }
}
