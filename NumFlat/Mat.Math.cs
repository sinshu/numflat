using System;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    public static class Mat
    {
        public static void Add<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] + sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void Sub<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] - sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void PointwiseMul<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] * sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void PointwiseDiv<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] / sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static unsafe void Mul(Mat<double> x, Vec<double> y, Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.ColCount != y.Count)
            {
                throw new ArgumentException("`y.Count` must match `x.ColCount`.");
            }

            if (destination.Count != x.RowCount)
            {
                throw new ArgumentException("`destination.Count` must match `x.RowCount`.");
            }

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemv(
                    Order.ColMajor,
                    Transpose.NoTrans,
                    x.RowCount, x.ColCount,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
                    pd, destination.Stride);
            }
        }

        public static unsafe void Mul(Mat<float> x, Mat<float> y, Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.ColCount != y.RowCount)
            {
                throw new ArgumentException("`y.RowCount` must match `x.ColCount`.");
            }

            var m = x.RowCount;
            var n = y.ColCount;
            var k = x.ColCount;

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sgemm(
                    Order.ColMajor,
                    Transpose.NoTrans, Transpose.NoTrans,
                    m, n, k,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    0.0F,
                    pd, destination.Stride);
            }
        }

        public static unsafe void Mul(Mat<double> x, Mat<double> y, Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.ColCount != y.RowCount)
            {
                throw new ArgumentException("`y.RowCount` must match `x.ColCount`.");
            }

            var m = x.RowCount;
            var n = y.ColCount;
            var k = x.ColCount;

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemm(
                    Order.ColMajor,
                    Transpose.NoTrans, Transpose.NoTrans,
                    m, n, k,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
                    pd, destination.Stride);
            }
        }

        public static unsafe void Mul(Mat<Complex> x, Mat<Complex> y, Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));

            if (x.ColCount != y.RowCount)
            {
                throw new ArgumentException("`y.RowCount` must match `x.ColCount`.");
            }

            var m = x.RowCount;
            var n = y.ColCount;
            var k = x.ColCount;

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Zgemm(
                    Order.ColMajor,
                    Transpose.NoTrans, Transpose.NoTrans,
                    m, n, k,
                    &one,
                    px, x.Stride,
                    py, y.Stride,
                    &zero,
                    pd, destination.Stride);
            }
        }
    }
}
