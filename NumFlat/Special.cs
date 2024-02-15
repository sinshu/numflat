using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides special functions.
    /// </summary>
    public static class Special
    {
        /// <summary>
        /// Gets a value that is very small compared to the scale of the given value.
        /// </summary>
        /// <param name="x">
        /// The value being compared.
        /// </param>
        /// <returns>
        /// A very small value compared to the given value.
        /// </returns>
        public static float Eps(float x)
        {
            if (float.IsInfinity(x) || float.IsNaN(x))
            {
                return float.NaN;
            }

            var bits = BitConverter.SingleToInt32Bits(x);

            if (x > 0)
            {
                bits += 1;
            }
            else if (x < 0)
            {
                bits -= 1;
            }
            else
            {
                return BitConverter.Int32BitsToSingle(1);
            }

            return BitConverter.Int32BitsToSingle(bits) - x;
        }

        /// <summary>
        /// Gets a value that is very small compared to the scale of the given value.
        /// </summary>
        /// <param name="x">
        /// The value being compared.
        /// </param>
        /// <returns>
        /// A very small value compared to the given value.
        /// </returns>
        public static double Eps(double x)
        {
            if (double.IsInfinity(x) || double.IsNaN(x))
            {
                return double.NaN;
            }

            var bits = BitConverter.DoubleToInt64Bits(x);

            if (x > 0)
            {
                bits += 1;
            }
            else if (x < 0)
            {
                bits -= 1;
            }
            else
            {
                return BitConverter.Int64BitsToDouble(1);
            }

            return BitConverter.Int64BitsToDouble(bits) - x;
        }

        /// <summary>
        /// Copy the lower triangular part to the upper triangular part to generate a Hermitian matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        public static void LowerTriangularToHermitianInplace(in Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var rows = x.Rows;
            var cols = x.Cols;
            for (var i = 0; i < x.RowCount - 1; i++)
            {
                var start = i + 1;
                var count = x.RowCount - start;
                if (count > 0)
                {
                    var src = cols[i].Subvector(start, count);
                    var dst = rows[i].Subvector(start, count);
                    src.CopyTo(dst);
                }
            }
        }

        /// <summary>
        /// Copy the lower triangular part to the upper triangular part to generate a Hermitian matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        public static void LowerTriangularToHermitianInplace(in Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var rows = x.Rows;
            var cols = x.Cols;
            for (var i = 0; i < x.RowCount - 1; i++)
            {
                var start = i + 1;
                var count = x.RowCount - start;
                if (count > 0)
                {
                    var src = cols[i].Subvector(start, count);
                    var dst = rows[i].Subvector(start, count);
                    Vec.Conjugate(src, dst);
                }
            }
        }

        /// <summary>
        /// Reverts the order of the eigenvalues and corresponding eigenvectors.
        /// </summary>
        /// <param name="evd">
        /// The target EVD result to be modified.
        /// </param>
        /// <remarks>
        /// The EVD orders the eigenvalues in ascending order by default.
        /// Reversing the order of the eigenvalues is useful when the significant eigenvalues should come first.
        /// </remarks>
        public static void ReverseEigenValueOrder(EigenValueDecompositionDouble evd)
        {
            ThrowHelper.ThrowIfNull(evd, nameof(evd));

            evd.D.ReverseInplace();
            foreach (var row in evd.V.Rows)
            {
                row.ReverseInplace();
            }
        }

        /// <summary>
        /// Reverts the order of the eigenvalues and corresponding eigenvectors.
        /// </summary>
        /// <param name="gevd">
        /// The target GEVD result to be modified.
        /// </param>
        /// <remarks>
        /// The EVD orders the eigenvalues in ascending order by default.
        /// Reversing the order of the eigenvalues is useful when the significant eigenvalues should come first.
        /// </remarks>
        public static void ReverseEigenValueOrder(GeneralizedEigenValueDecompositionDouble gevd)
        {
            ThrowHelper.ThrowIfNull(gevd, nameof(gevd));

            gevd.D.ReverseInplace();
            foreach (var row in gevd.V.Rows)
            {
                row.ReverseInplace();
            }
        }
    }
}
