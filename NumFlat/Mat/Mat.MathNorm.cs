using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Computes the Frobenius norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The Frobenius norm.
        /// </returns>
        public static float FrobeniusNorm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sum = 0.0F;
            var span = x.Memory.Span;
            var offset = 0;
            while (offset < span.Length)
            {
                var position = offset;
                var end = offset + x.RowCount;
                while (position < end)
                {
                    var value = span[position];
                    sum += value * value;
                    position++;
                }
                offset += x.Stride;
            }

            return MathF.Sqrt(sum);
        }

        /// <summary>
        /// Computes the Frobenius norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The Frobenius norm.
        /// </returns>
        public static double FrobeniusNorm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sum = 0.0;
            var span = x.Memory.Span;
            var offset = 0;
            while (offset < span.Length)
            {
                var position = offset;
                var end = offset + x.RowCount;
                while (position < end)
                {
                    var value = span[position];
                    sum += value * value;
                    position++;
                }
                offset += x.Stride;
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Computes the Frobenius norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The Frobenius norm.
        /// </returns>
        public static double FrobeniusNorm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var sum = 0.0;
            var span = x.Memory.Span;
            var offset = 0;
            while (offset < span.Length)
            {
                var position = offset;
                var end = offset + x.RowCount;
                while (position < end)
                {
                    sum += span[position].MagnitudeSquared();
                    position++;
                }
                offset += x.Stride;
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static float L1Norm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the L1 norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The L1 norm.
        /// </returns>
        public static double L1Norm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var col in x.Cols)
            {
                var norm = col.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
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

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static float InfinityNorm(in this Mat<float> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0F;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }

        /// <summary>
        /// Computes the infinity norm.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The infinity norm.
        /// </returns>
        public static double InfinityNorm(in this Mat<Complex> x)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var max = 0.0;
            foreach (var row in x.Rows)
            {
                var norm = row.L1Norm();
                if (norm > max)
                {
                    max = norm;
                }
            }
            return max;
        }
    }
}
