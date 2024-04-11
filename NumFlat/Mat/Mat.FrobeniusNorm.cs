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
    }
}
