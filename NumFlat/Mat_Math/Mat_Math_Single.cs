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
        /// 'y.Count' must match 'x.ColCount'.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix-and-vector multiplication.
        /// 'destination.Count' must match 'x.RowCount'.
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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sgemv(
                    Order.ColMajor,
                    transX,
                    x.RowCount, x.ColCount,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    0.0F,
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
        /// The destination of the result of the matrix multiplication.
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

            var transX = transposeX ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var transY = transposeY ? OpenBlasSharp.Transpose.Trans : OpenBlasSharp.Transpose.NoTrans;
            var (m, n, k) = GetMulArgs(x, transposeX, y, transposeY, destination);

            fixed (float* px = x.Memory.Span)
            fixed (float* py = y.Memory.Span)
            fixed (float* pd = destination.Memory.Span)
            {
                Blas.Sgemm(
                    Order.ColMajor,
                    transX, transY,
                    m, n, k,
                    1.0F,
                    px, x.Stride,
                    py, y.Stride,
                    0.0F,
                    pd, destination.Stride);
            }
        }

        /// <summary>
        /// Computes the determinant of a matrix, det(X).
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <returns>
        /// The determinant of the matrix.
        /// </returns>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe float Determinant(in Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            var xLength = x.RowCount * x.ColCount;
            using var xBuffer = MemoryPool<float>.Shared.Rent(xLength);
            var xCopy = new Mat<float>(x.RowCount, x.ColCount, x.RowCount, xBuffer.Memory.Slice(0, xLength));
            x.CopyTo(xCopy);

            using var pivBuffer = MemoryPool<int>.Shared.Rent(xCopy.RowCount);
            var piv = pivBuffer.Memory.Span;

            fixed (float* px = xCopy.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Sgetrf(
                    MatrixLayout.ColMajor,
                    xCopy.RowCount, xCopy.ColCount,
                    px, xCopy.Stride,
                    ppiv);
            }

            var determinant = 1.0F;
            for (var i = 0; i < xCopy.RowCount; i++)
            {
                if (piv[i] - 1 == i)
                {
                    determinant *= xCopy[i, i];
                }
                else
                {
                    determinant *= -xCopy[i, i];
                }
            }

            return determinant;
        }

        /// <summary>
        /// Computes a matrix inversion, X^-1.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the matrix inversion.
        /// </param>
        /// <exception cref="LapackException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static unsafe void Inverse(in Mat<float> x, in Mat<float> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("'x' must be a square matrix.");
            }

            x.CopyTo(destination);

            using var pivBuffer = MemoryPool<int>.Shared.Rent(destination.RowCount);
            var piv = pivBuffer.Memory.Span;

            fixed (float* pd = destination.Memory.Span)
            fixed (int* ppiv = piv)
            {
                var info = Lapack.Sgetrf(
                    MatrixLayout.ColMajor,
                    destination.RowCount, destination.ColCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Sgetrf), (int)info);
                }

                info = Lapack.Sgetri(
                    MatrixLayout.ColMajor,
                    destination.RowCount,
                    pd, destination.Stride,
                    ppiv);
                if (info != LapackInfo.None)
                {
                    throw new LapackException("The matrix is ill-conditioned.", nameof(Lapack.Sgetri), (int)info);
                }
            }
        }

        /// <summary>
        /// Gets the rank of the matrix.
        /// </summary>
        /// <param name="x">
        /// The matrix.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        /// <exception cref="LapackException">
        /// The SVD computation did not converge.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static int Rank(in this Mat<float> x, float tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sLength = Math.Min(x.RowCount, x.ColCount);
            using var sBuffer = MemoryPool<float>.Shared.Rent(sLength);
            var s = new Vec<float>(sBuffer.Memory.Slice(0, sLength));
            SingularValueDecompositionSingle.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (float.IsNaN(tolerance))
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
        /// The matrix.
        /// </param>
        /// <returns>
        /// The rank of the matrix.
        /// </returns>
        /// <exception cref="LapackException">
        /// The SVD computation did not converge.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static int Rank(in this Mat<float> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(x, float.NaN);
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pseudo inversion.
        /// </param>
        /// <param name="tolerance">
        /// Singular values below this threshold will be ignored.
        /// </param>
        /// <exception cref="LapackException">
        /// The SVD computation did not converge.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
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

            var sLength = Math.Min(a.RowCount, a.ColCount);
            using var sBuffer = MemoryPool<float>.Shared.Rent(sLength);
            var s = new Vec<float>(sBuffer.Memory.Slice(0, sLength));

            var uLength = a.RowCount * a.RowCount;
            using var uBuffer = MemoryPool<float>.Shared.Rent(uLength);
            var u = new Mat<float>(a.RowCount, a.RowCount, a.RowCount, uBuffer.Memory.Slice(0, uLength));

            var vtLength = a.ColCount * a.ColCount;
            using var vtBuffer = MemoryPool<float>.Shared.Rent(vtLength);
            var vt = new Mat<float>(a.ColCount, a.ColCount, a.ColCount, vtBuffer.Memory.Slice(0, vtLength));

            SingularValueDecompositionSingle.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (float.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(a.RowCount, a.RowCount);
            }

            var tmpLength = a.ColCount * a.RowCount;
            using var tmpBuffer = MemoryPool<float>.Shared.Rent(tmpLength);
            var tmp = new Mat<float>(a.ColCount, a.RowCount, a.ColCount, tmpBuffer.Memory.Slice(0, tmpLength));
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
        /// Computes a pseudo inverse of the matrix A.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="destination">
        /// The destination of the result of the pseudo inversion.
        /// </param>
        /// <exception cref="LapackException">
        /// The SVD computation did not converge.
        /// </exception>
        /// <remarks>
        /// This method internally uses '<see cref="MemoryPool{T}.Shared"/>' to allocate buffer.
        /// </remarks>
        public static void PseudoInverse(in Mat<float> a, in Mat<float> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, float.NaN);
        }
    }
}
