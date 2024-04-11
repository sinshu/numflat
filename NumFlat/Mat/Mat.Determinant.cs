using System;
using System.Buffers;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
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

            int sign;
            fixed (double* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var ftmp = tmp.GetUnsafeFastIndexer();
            var determinant = (double)sign;
            for (var i = 0; i < tmp.RowCount; i++)
            {
                determinant *= ftmp[i, i];
            }
            return determinant;
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

            int sign;
            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var ftmp = tmp.GetUnsafeFastIndexer();
            var determinant = (Complex)sign;
            for (var i = 0; i < tmp.RowCount; i++)
            {
                determinant *= ftmp[i, i];
            }
            return determinant;
        }
    }
}
