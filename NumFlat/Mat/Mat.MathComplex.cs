using System;
using System.Buffers;
using System.Numerics;
using OpenBlasSharp;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes a matrix-and-vector multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// This vector is interpreted as a column vector.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix-and-vector multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="conjugateX">
        /// If true, the matrix X is treated as conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<Complex> x, in Vec<Complex> y, in Vec<Complex> destination, bool transposeX, bool conjugateX)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (transposeX)
            {
                if (y.Count != x.RowCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.RowCount'.");
                }

                if (destination.Count != x.ColCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.ColCount'.");
                }
            }
            else
            {
                if (y.Count != x.ColCount)
                {
                    throw new ArgumentException("'y.Count' must match 'x.ColCount'.");
                }

                if (destination.Count != x.RowCount)
                {
                    throw new ArgumentException("'destination.Count' must match 'x.RowCount'.");
                }
            }

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            if (conjugateX)
            {
                transX = transX == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Zgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    &one,
                    px, x.Stride,
                    py, y.Stride,
                    &zero,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes a matrix multiplication, X * Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="conjugateX">
        /// If true, the matrix X is treated as conjugated.
        /// </param>
        /// <param name="transposeY">
        /// If true, the matrix Y is treated as transposed.
        /// </param>
        /// <param name="conjugateY">
        /// If true, the matrix Y is treated as conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<Complex> x, in Mat<Complex> y, in Mat<Complex> destination, bool transposeX, bool conjugateX, bool transposeY, bool conjugateY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);
            if (conjugateX)
            {
                transX = transX == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }
            if (conjugateY)
            {
                transY = transY == OpenBlasSharp.Transpose.Trans ? OpenBlasSharp.Transpose.ConjTrans : OpenBlasSharp.Transpose.ConjNoTrans;
            }

            var one = Complex.One;
            var zero = Complex.Zero;

            fixed (Complex* px = x.Memory.Span)
            fixed (Complex* py = y.Memory.Span)
            fixed (Complex* pd = destination.Memory.Span)
            {
                Blas.Zgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    &one,
                    px, x.Stride,
                    py, y.Stride,
                    &zero,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes the determinant of a matrix, det(X).
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The determinant of the matrix.
        /// </returns>
        public static unsafe Complex Determinant(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            using var utmp = TemporalMatrix.CopyFrom(x);
            ref readonly var tmp = ref utmp.Item;

            using var upiv = MemoryPool<int>.Shared.Rent(tmp.RowCount);
            var piv = upiv.Memory.Span;

            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Zgetrf(
                    MatrixLayout.ColMajor,
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    ppiv);
            }

            var ftmp = tmp.GetUnsafeFastIndexer();
            var determinant = Complex.One;
            for (var i = 0; i < tmp.RowCount; i++)
            {
                if (piv[i] - 1 == i)
                {
                    determinant *= ftmp[i, i];
                }
                else
                {
                    determinant *= -ftmp[i, i];
                }
            }

            return determinant;
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix inversion.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Inverse(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (Complex* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Zgetrf(
                    MatrixLayout.ColMajor,
                    destination.RowCount, destination.ColCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetrf), (int)info);
                }

                info = Lapack.Zgetri(
                    MatrixLayout.ColMajor,
                    destination.RowCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Zgetri), (int)info);
                }
            }
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be replaced with zero.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<Complex> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var us = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var s = ref us.Item;

            SingularValueDecompositionComplex.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.RowCount);
            }

            var rank = 0;
            foreach (var value in s.FastEnumerate())
            {
                if (value > tolerance)
                {
                    rank++;
                }
            }

            return rank;
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<Complex> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(x, double.NaN);
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix, pinv(A).
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination of the pseudo inversion.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be replaced with zero.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static void PseudoInverse(in Mat<Complex> a, in Mat<Complex> destination, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != a.ColCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'a.ColCount'.");
            }

            if (destination.ColCount != a.RowCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'a.RowCount'.");
            }

            using var us = new TemporalVector<double>(Math.Min(a.RowCount, a.ColCount));
            ref readonly var s = ref us.Item;

            using var uu = new TemporalMatrix<Complex>(a.RowCount, a.RowCount);
            ref readonly var u = ref uu.Item;

            using var uvt = new TemporalMatrix<Complex>(a.ColCount, a.ColCount);
            ref readonly var vt = ref uvt.Item;

            SingularValueDecompositionComplex.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(a.RowCount, a.RowCount);
            }

            using var utmp = new TemporalMatrix<Complex>(a.ColCount, a.RowCount);
            ref readonly var tmp = ref utmp.Item;
            tmp.Clear();

            if (a.RowCount >= a.ColCount)
            {
                var tmpCols = tmp.Cols;
                var vtRows = vt.Rows;
                for (var i = 0; i < s.Count; i++)
                {
                    if (s[i] > tolerance)
                    {
                        ConjugateDiv(vtRows[i], s[i], tmpCols[i]);
                    }
                }
                Mat.Mul(tmp, u, destination, false, false, true, true);
            }
            else
            {
                var tmpRows = tmp.Rows;
                var uCols = u.Cols;
                for (var i = 0; i < s.Count; i++)
                {
                    if (s[i] > tolerance)
                    {
                        ConjugateDiv(uCols[i], s[i], tmpRows[i]);
                    }
                }
                Mat.Mul(vt, tmp, destination, true, true, false, false);
            }
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix, pinv(A).
        /// </summary>
        /// <param name="a">
        /// The matrix to be inverted.
        /// </param>
        /// <param name="destination">
        /// The destination the pseudo inversion.
        /// </param>
        /// <exception cref="LapackException">
        /// Failed to compute the SVD.
        /// </exception>
        public static void PseudoInverse(in Mat<Complex> a, in Mat<Complex> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, double.NaN);
        }

        /// <summary>
        /// Conjugates the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be conjugated.
        /// </param>
        /// <param name="destination">
        /// The destination of the complex matrix conjugation.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// To efficiently perform matrix multiplication with matrix conjugation,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void Conjugate(in Mat<Complex> x, in Mat<Complex> destination)
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
                    sd[pd] = sx[px].Conjugate();
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the Hermitian transpose of a complex matrix, X^H.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be transposed.
        /// </param>
        /// <param name="destination">
        /// The destination of the Hermitian transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// Since in-place transposition is not supported,
        /// <paramref name="x"/> and <paramref name="destination"/> must be different.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void ConjugateTranspose(in Mat<Complex> x, in Mat<Complex> destination)
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
                    sd[pd] = sx[px].Conjugate();
                    px += x.Stride;
                    pd++;
                }
            }
        }

        private static void ConjugateDiv(in Vec<Complex> x, double y, in Vec<Complex> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Conjugate() / y;
                px += x.Stride;
                pd += destination.Stride;
            }
        }
    }
}
