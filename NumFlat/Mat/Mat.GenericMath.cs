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
        /// The destination of the matrix addition.
        /// </param>
        /// <remarks>
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
        /// Computes a pointwise matrix-and-scalar addition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the pointwise matrix-and-scalar addition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Add<T>(in Mat<T> x, T y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

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
                    sd[pd] = sx[px] + y;
                    px++;
                    pd++;
                }
                ox += x.Stride;
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
        /// The destination of the matrix subtraction.
        /// </param>
        /// <remarks>
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
        /// Computes a pointwise matrix-and-scalar subtraction.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the pointwise matrix-and-scalar subtraction.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Sub<T>(in Mat<T> x, T y, in Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

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
                    sd[pd] = sx[px] - y;
                    px++;
                    pd++;
                }
                ox += x.Stride;
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
        /// The destination of the matrix-and-scalar multiplication.
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
        /// The destination of the matrix-and-scalar division.
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
        /// Computes a pointwise multiplication of matrices, X .* Y.
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
        /// The destination the pointwise multiplication.
        /// </param>
        /// <remarks>
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
        /// Computes a pointwise division of matrices, X ./ Y.
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
        /// The destination of the pointwise division.
        /// </param>
        /// <remarks>
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
        /// The destination of the matrix transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// This method does not support in-place transposition.
        /// To transpose a matrix in-place, use <see cref="MatrixInplaceOperations.TransposeInplace{T}(in Mat{T})"/> instead.
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
        /// The target matrix.
        /// </param>
        /// <returns></returns>
        public static T Trace<T>(in this Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var fx = x.GetUnsafeFastIndexer();
            var trace = T.Zero;
            for (var i = 0; i < x.RowCount; i++)
            {
                trace += fx[i, i];
            }

            return trace;
        }

        /// <summary>
        /// Applies a function to each value of the source matrix.
        /// </summary>
        /// <typeparam name="TSource">
        /// The source type.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The destination type.
        /// </typeparam>
        /// <param name="source">
        /// The source matrix.
        /// </param>
        /// <param name="func">
        /// The function to be applied.
        /// </param>
        /// <param name="destination">
        /// The destination where the results of the function application are stored.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Map<TSource, TResult>(in Mat<TSource> source, Func<TSource, TResult> func, in Mat<TResult> destination) where TSource : unmanaged, INumberBase<TSource> where TResult : unmanaged, INumberBase<TResult>
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (source.RowCount != destination.RowCount || source.ColCount != destination.ColCount)
            {
                throw new ArgumentException("The matrices must have the same dimensions.");
            }

            var ss = source.Memory.Span;
            var sd = destination.Memory.Span;
            var os = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var ps = os;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = func(ss[ps]);
                    ps++;
                    pd++;
                }
                os += source.Stride;
                od += destination.Stride;
            }
        }
    }
}
