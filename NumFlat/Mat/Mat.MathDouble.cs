using System;
using System.Buffers;
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<double> x, in Vec<double> y, in Vec<double> destination, bool transposeX)
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

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
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
        /// The destination the matrix multiplication.
        /// </param>
        /// <param name="transposeX">
        /// If true, the matrix X is treated as transposed.
        /// </param>
        /// <param name="transposeY">
        /// If true, the matrix Y is treated as transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static unsafe void Mul(in Mat<double> x, in Mat<double> y, in Mat<double> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

            fixed (double* px = x.Memory.Span)
            fixed (double* py = y.Memory.Span)
            fixed (double* pd = destination.Memory.Span)
            {
                Blas.Dgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    1.0,
                    px, x.Stride,
                    py, y.Stride,
                    0.0,
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
        public static unsafe double Determinant(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            using var utmp = TemporalMatrix.CopyFrom(x);
            ref readonly var tmp = ref utmp.Item;

            using var upiv = MemoryPool<int>.Shared.Rent(tmp.RowCount);
            var piv = upiv.Memory.Span;

            fixed (double* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    tmp.RowCount, tmp.ColCount,
                    ptmp, tmp.Stride,
                    ppiv);
            }

            var ftmp = tmp.GetUnsafeFastIndexer();
            var determinant = 1.0;
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
        public static unsafe void Inverse(in Mat<double> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (double* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Dgetrf(
                    MatrixLayout.ColMajor,
                    destination.RowCount, destination.ColCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetrf), (int)info);
                }

                info = Lapack.Dgetri(
                    MatrixLayout.ColMajor,
                    destination.RowCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Dgetri), (int)info);
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
        public static int Rank(in this Mat<double> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var us = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var s = ref us.Item;

            SingularValueDecompositionDouble.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.RowCount);
            }

            var rank = 0;
            foreach (var value in s)
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
        public static int Rank(in this Mat<double> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return x.Rank(double.NaN);
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
        public static void PseudoInverse(in Mat<double> a, in Mat<double> destination, double tolerance)
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

            using var uu = new TemporalMatrix<double>(a.RowCount, a.RowCount);
            ref readonly var u = ref uu.Item;

            using var uvt = new TemporalMatrix<double>(a.ColCount, a.ColCount);
            ref readonly var vt = ref uvt.Item;

            SingularValueDecompositionDouble.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(a.RowCount, a.RowCount);
            }

            using var utmp = new TemporalMatrix<double>(a.ColCount, a.RowCount);
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
                        Vec.Div(vtRows[i], s[i], tmpCols[i]);
                    }
                }
                Mat.Mul(tmp, u, destination, false, true);
            }
            else
            {
                var tmpRows = tmp.Rows;
                var uCols = u.Cols;
                for (var i = 0; i < s.Count; i++)
                {
                    if (s[i] > tolerance)
                    {
                        Vec.Div(uCols[i], s[i], tmpRows[i]);
                    }
                }
                Mat.Mul(vt, tmp, destination, true, false);
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
        public static void PseudoInverse(in Mat<double> a, in Mat<double> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, double.NaN);
        }
    }
}
