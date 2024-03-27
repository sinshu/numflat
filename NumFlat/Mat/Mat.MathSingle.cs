using System;
using System.Buffers;
using MatFlat;

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
        public static unsafe void Mul(in Mat<float> x, in Vec<float> y, in Vec<float> destination, bool transposeX)
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

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.MulMatVec(transX, x.RowCount, x.ColCount, px, x.Stride, py, y.Stride, pd, destination.Stride);
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
        public static unsafe void Mul(in Mat<float> x, in Mat<float> y, in Mat<float> destination, bool transposeX, bool transposeY)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var transX = transposeX ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var transY = transposeY ? MatFlat.Transpose.Trans : MatFlat.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.MulMatMat(transX, transY, m, n, k, px, x.Stride, py, y.Stride, pd, destination.Stride);
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
        public static unsafe float Determinant(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            using var utmp = TemporalMatrix.CopyFrom(x);
            ref readonly var tmp = ref utmp.Item;

            using var upiv = MemoryPool<int>.Shared.Rent(tmp.RowCount);
            var piv = upiv.Memory.Span;

            int sign;
            fixed (float* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var ftmp = tmp.GetUnsafeFastIndexer();
            var determinant = (float)sign;
            for (var i = 0; i < tmp.RowCount; i++)
            {
                determinant *= ftmp[i, i];
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
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        public static unsafe void Inverse(in Mat<float> x, in Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            x.CopyTo(destination);

            using var upiv = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = upiv.Memory.Span;

            fixed (float* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                Factorization.Lu(destination.RowCount, destination.RowCount, pd, destination.Stride, ppiv);
                Factorization.LuInverse(destination.RowCount, pd, destination.Stride, ppiv);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<float> x, float tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var us = new TemporalVector<float>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var s = ref us.Item;
            var fs = s.GetUnsafeFastIndexer();

            SingularValueDecompositionSingle.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (float.IsNaN(tolerance))
            {
                tolerance = Special.Eps(fs[0]) * Math.Max(x.RowCount, x.ColCount);
            }

            var rank = 0;
            foreach (var value in fs)
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<float> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(x, float.NaN);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static void PseudoInverse(in Mat<float> a, in Mat<float> destination, float tolerance)
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

            using var us = new TemporalVector<float>(Math.Min(a.RowCount, a.ColCount));
            ref readonly var s = ref us.Item;
            var fs = s.GetUnsafeFastIndexer();

            using var uu = new TemporalMatrix<float>(a.RowCount, a.RowCount);
            ref readonly var u = ref uu.Item;

            using var uvt = new TemporalMatrix<float>(a.ColCount, a.ColCount);
            ref readonly var vt = ref uvt.Item;

            SingularValueDecompositionSingle.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (float.IsNaN(tolerance))
            {
                tolerance = Special.Eps(fs[0]) * Math.Max(a.RowCount, a.ColCount);
            }

            using var utmp = new TemporalMatrix<float>(a.ColCount, a.RowCount);
            ref readonly var tmp = ref utmp.Item;
            tmp.Clear();

            if (a.RowCount >= a.ColCount)
            {
                var tmpCols = tmp.Cols;
                var vtRows = vt.Rows;
                for (var i = 0; i < s.Count; i++)
                {
                    if (fs[i] > tolerance)
                    {
                        Vec.Div(vtRows[i], fs[i], tmpCols[i]);
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
                    if (fs[i] > tolerance)
                    {
                        Vec.Div(uCols[i], fs[i], tmpRows[i]);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static void PseudoInverse(in Mat<float> a, in Mat<float> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, float.NaN);
        }
    }
}
