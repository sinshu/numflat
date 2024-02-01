using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides low-level matrix operations.
    /// </summary>
    public static partial class Mat
    {
        /// <summary>
        /// Computes a matrix addition, X + Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix addition.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Add<T>(in Mat<T> x, in Mat<T> y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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

        /// <summary>
        /// Computes a matrix subtraction, X - Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix subtraction.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Sub<T>(in Mat<T> x, in Mat<T> y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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

        /// <summary>
        /// Computes a matrix-and-scalar multiplication, X * y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-scalar multiplication.
        /// The dimensions of <paramref name="destination"/> must match <paramref name="x"/>.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Mul<T>(in Mat<T> x, T y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] * y;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a matrix-and-scalar division, X * y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-scalar division.
        /// The dimensions of <paramref name="destination"/> must match <paramref name="x"/>.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Div<T>(in Mat<T> x, T y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] / y;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes a pointwise-multiplication of matrices, X .* Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-multiplication.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMul<T>(in Mat<T> x, in Mat<T> y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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

        /// <summary>
        /// Computes a pointwise-division of matrices, X ./ Y.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pointwise-division.
        /// </param>
        /// <remarks>
        /// The dimensions of the matrices must match.
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDiv<T>(in Mat<T> x, in Mat<T> y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, y, destination);

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

        /// <summary>
        /// Computes a matrix transposition, X^T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// Since in-place transposition is not supported,
        /// <paramref name="x"/> and <paramref name="destination"/> must be different.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(in Mat{double}, in Mat{double}, in Mat{double}, bool, bool)"/>.
        /// </remarks>
        public static void Transpose<T>(in Mat<T> x, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.ColCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.ColCount'.");
            }

            if (destination.ColCount != x.RowCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.RowCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var px = col;
                var pd = destination.Stride * col;
                var end = pd + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px];
                    px += x.Stride;
                    pd++;
                }
            }
        }

        /// <summary>
        /// Computes the trace of a matrix, tr(X).
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns></returns>
        public static T Trace<T>(this in Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            var trace = T.Zero;
            for (var i = 0; i < x.RowCount; i++)
            {
                trace += x[i, i];
            }

            return trace;
        }

        private static (int m, int n, int k) GetMulArgs<T>(in Mat<T> x, bool transposeX, in Mat<T> y, bool transposeY, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            var xRowCount = (count: x.RowCount, name: "x.RowCount");
            var xColCount = (count: x.ColCount, name: "x.ColCount");
            var yRowCount = (count: y.RowCount, name: "y.RowCount");
            var yColCount = (count: y.ColCount, name: "y.ColCount");

            if (transposeX)
            {
                (xRowCount, xColCount) = (xColCount, xRowCount);
            }

            if (transposeY)
            {
                (yRowCount, yColCount) = (yColCount, yRowCount);
            }

            if (yRowCount.count != xColCount.count)
            {
                throw new ArgumentException($"'{yRowCount.name}' must match '{xColCount.name}'.");
            }

            var m = xRowCount;
            var n = yColCount;
            var k = xColCount;

            if (destination.RowCount != m.count)
            {
                throw new ArgumentException($"The dimensions of 'destination' must match '{m.name}'.");
            }

            if (destination.ColCount != n.count)
            {
                throw new ArgumentException($"The dimensions of 'destination' must match '{n.name}'.");
            }

            return (m.count, n.count, k.count);
        }
    }
}
