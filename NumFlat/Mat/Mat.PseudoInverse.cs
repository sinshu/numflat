using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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

        /// <summary>
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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
            var fs = s.GetUnsafeFastIndexer();

            using var uu = new TemporalMatrix<double>(a.RowCount, a.RowCount);
            ref readonly var u = ref uu.Item;

            using var uvt = new TemporalMatrix<double>(a.ColCount, a.ColCount);
            ref readonly var vt = ref uvt.Item;

            SingularValueDecompositionDouble.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(fs[0]) * Math.Max(a.RowCount, a.ColCount);
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
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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
        public static void PseudoInverse(in Mat<double> a, in Mat<double> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, double.NaN);
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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
            var fs = s.GetUnsafeFastIndexer();

            using var uu = new TemporalMatrix<Complex>(a.RowCount, a.RowCount);
            ref readonly var u = ref uu.Item;

            using var uvt = new TemporalMatrix<Complex>(a.ColCount, a.ColCount);
            ref readonly var vt = ref uvt.Item;

            SingularValueDecompositionComplex.Decompose(a, s, u, vt);

            // If tolerance is NaN, set the tolerance by the Math.NET's method.
            if (double.IsNaN(tolerance))
            {
                tolerance = Special.Eps(fs[0]) * Math.Max(a.RowCount, a.ColCount);
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
                    if (fs[i] > tolerance)
                    {
                        ConjugateDiv(vtRows[i], fs[i], tmpCols[i]);
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
                    if (fs[i] > tolerance)
                    {
                        ConjugateDiv(uCols[i], fs[i], tmpRows[i]);
                    }
                }
                Mat.Mul(vt, tmp, destination, true, true, false, false);
            }
        }

        /// <summary>
        /// Computes a pseudo inverse of the matrix, <c>pinv(A)</c>.
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
        public static void PseudoInverse(in Mat<Complex> a, in Mat<Complex> destination)
        {
            // Set NaN to tolerance to set the tolerance automatically.
            PseudoInverse(a, destination, double.NaN);
        }
    }
}
