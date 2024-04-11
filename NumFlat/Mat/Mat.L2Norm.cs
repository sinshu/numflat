using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static float L2Norm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var utmp = new TemporalVector<float>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var tmp = ref utmp.Item;
            SingularValueDecompositionSingle.GetSingularValues(x, tmp);
            return tmp[0];
        }

        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static double L2Norm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var utmp = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var tmp = ref utmp.Item;
            SingularValueDecompositionDouble.GetSingularValues(x, tmp);
            return tmp[0];
        }

        /// <summary>
        /// Computes the L2 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L2 norm.
        /// </returns>
        public static double L2Norm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            using var utmp = new TemporalVector<double>(Math.Min(x.RowCount, x.ColCount));
            ref readonly var tmp = ref utmp.Item;
            SingularValueDecompositionComplex.GetSingularValues(x, tmp);
            return tmp[0];
        }
    }
}
