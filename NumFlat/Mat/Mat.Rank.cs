using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
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

            SingularValueDecompositionSingle.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (float.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.ColCount);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<float> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(x, float.NaN);
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
        public static int Rank(in this Mat<double> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var us = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var s = ref us.Item;

            SingularValueDecompositionDouble.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.ColCount);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<double> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return x.Rank(double.NaN);
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
        public static int Rank(in this Mat<Complex> x, double tolerance)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var us = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var s = ref us.Item;

            SingularValueDecompositionComplex.GetSingularValues(x, s);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(s[0]) * Math.Max(x.RowCount, x.ColCount);
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
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static int Rank(in this Mat<Complex> x)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            return Rank(x, double.NaN);
        }
    }
}
