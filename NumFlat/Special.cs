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
    }
}
