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
        /// Computes the product of diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The product of diagonal elements.
        /// </returns>
        public static float DiagonalProduct(in Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var product = 1.0F;
            while (offset < span.Length)
            {
                product *= span[offset + row];
                row++;
                offset += x.Stride;
            }

            return product;
        }

        /// <summary>
        /// Computes the product of diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The product of diagonal elements.
        /// </returns>
        public static double DiagonalProduct(in Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var product = 1.0;
            while (offset < span.Length)
            {
                product *= span[offset + row];
                row++;
                offset += x.Stride;
            }

            return product;
        }

        /// <summary>
        /// Computes the product of diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The product of diagonal elements.
        /// </returns>
        /// <remarks>
        /// For complex values, only the real parts are referenced.
        /// </remarks>
        public static double DiagonalProduct(in Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var product = 1.0;
            while (offset < span.Length)
            {
                product *= span[offset + row].Real;
                row++;
                offset += x.Stride;
            }

            return product;
        }

        /// <summary>
        /// Computes the sum of log diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The sum of log diagonal elements.
        /// </returns>
        public static float DiagonalLogSum(in Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var sum = 0.0F;
            while (offset < span.Length)
            {
                sum += MathF.Log(span[offset + row]);
                row++;
                offset += x.Stride;
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of log diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The sum of log diagonal elements.
        /// </returns>
        public static double DiagonalLogSum(in Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var sum = 0.0;
            while (offset < span.Length)
            {
                sum += Math.Log(span[offset + row]);
                row++;
                offset += x.Stride;
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of log diagonal elements of a matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The sum of log diagonal elements.
        /// </returns>
        /// <remarks>
        /// For complex values, only the real parts are referenced.
        /// </remarks>
        public static double DiagonalLogSum(in Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            var sum = 0.0;
            while (offset < span.Length)
            {
                sum += Math.Log(span[offset + row].Real);
                row++;
                offset += x.Stride;
            }

            return sum;
        }

        /// <summary>
        /// Copies the lower triangular part to the upper triangular part to generate a Hermitian matrix.
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
        /// Copies the lower triangular part to the upper triangular part to generate a Hermitian matrix.
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
        /// Increases the diagonal elements of the matrix.
        /// This is useful to regularize a covariance matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <param name="value">
        /// The amount of the increment.
        /// </param>
        public static void IncreaseDiagonalElementsInplace(in Mat<double> x, double value)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfNonSquare(x, nameof(x));

            var span = x.Memory.Span;
            var offset = 0;
            var row = 0;
            while (offset < span.Length)
            {
                span[offset + row] += value;
                row++;
                offset += x.Stride;
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
