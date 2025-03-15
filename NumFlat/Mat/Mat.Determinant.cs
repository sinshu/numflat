using System;
using System.Buffers;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes the determinant of a matrix, <c>det(X)</c>.
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

            using var upiv = new TemporalArray<int>(tmp.RowCount);
            var piv = upiv.Item;

            int sign;
            fixed (float* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var determinant = (float)sign;
            foreach (var value in tmp.EnumerateDiagonalElements())
            {
                determinant *= value;
            }
            return determinant;
        }

        /// <summary>
        /// Computes the determinant of a matrix, <c>det(X)</c>.
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

            using var upiv = new TemporalArray<int>(tmp.RowCount);
            var piv = upiv.Item;

            int sign;
            fixed (double* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var determinant = (double)sign;
            foreach (var value in tmp.EnumerateDiagonalElements())
            {
                determinant *= value;
            }
            return determinant;
        }

        /// <summary>
        /// Computes the determinant of a matrix, <c>det(X)</c>.
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

            using var upiv = new TemporalArray<int>(tmp.RowCount);
            var piv = upiv.Item;

            int sign;
            fixed (Complex* ptmp = tmp.Memory.Span)
            fixed (int* ppiv = piv)
            {
                sign = Factorization.Lu(tmp.RowCount, tmp.RowCount, ptmp, tmp.Stride, ppiv);
            }

            var determinant = (Complex)sign;
            foreach (var value in tmp.EnumerateDiagonalElements())
            {
                determinant *= value;
            }
            return determinant;
        }
    }
}
