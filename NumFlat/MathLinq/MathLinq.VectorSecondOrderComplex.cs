using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean vector from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean vector.
        /// </param>
        public static void Mean(IEnumerable<Vec<Complex>> xs, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the source vectors must have the same length as the destination.");
                }

                destination.AddInplace(x);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            destination.DivInplace(count);
        }

        /// <summary>
        /// Computes the mean vector from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <returns>
        /// The mean vector.
        /// </returns>
        public static Vec<Complex> Mean(this IEnumerable<Vec<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Vec<Complex>();

            var count = 0;
            foreach (var x in xs)
            {
                if (destination.Count == 0)
                {
                    if (x.Count == 0)
                    {
                        new ArgumentException("Empty vectors are not allowed.");
                    }

                    destination = new Vec<Complex>(x.Count);
                }

                if (x.Count != destination.Count)
                {
                    throw new ArgumentException("All the vectors must have the same length.");
                }

                destination.AddInplace(x);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            destination.DivInplace(count);

            return destination;
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean vector of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the pointwise variance.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        public static void Variance(IEnumerable<Vec<Complex>> xs, in Vec<Complex> mean, in Vec<double> destination, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.Count != mean.Count)
            {
                throw new ArgumentException("The length of the destination must match the length of the mean vector.");
            }

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            destination.Clear();
            var count = 0;

            foreach (var x in xs)
            {
                if (x.Count != mean.Count)
                {
                    throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                }

                AccumulateVariance(x, mean, destination);
                count++;
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            destination.DivInplace(count - ddof);
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="mean">
        /// The pre-computed mean vector of the source vectors.
        /// </param>
        /// <param name="destination">
        /// The destination of the covariance matrix.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        public static unsafe void Covariance(IEnumerable<Vec<Complex>> xs, in Vec<Complex> mean, in Mat<Complex> destination, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(mean, nameof(mean));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != mean.Count)
            {
                throw new ArgumentException("The number of rows of the destination must match the length of the mean vector.");
            }

            if (destination.ColCount != mean.Count)
            {
                throw new ArgumentException("The number of columns of the destination must match the length of the mean vector.");
            }

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var one = Complex.One;

            using var ucentered = new TemporalVector<Complex>(mean.Count);
            ref readonly var centered = ref ucentered.Item;

            destination.Clear();
            var count = 0;

            fixed (Complex* pd = destination.Memory.Span)
            fixed (Complex* pc = centered.Memory.Span)
            {
                foreach (var x in xs)
                {
                    if (x.Count != mean.Count)
                    {
                        throw new ArgumentException("All the source vectors must have the same length as the mean vector.");
                    }

                    Vec.Sub(x, mean, centered);
                    AccumulateCovariance(centered, destination);
                    count++;
                }
            }

            if (count - ddof <= 0)
            {
                throw new ArgumentException("The number of source vectors is not sufficient.");
            }

            var i = 0;
            foreach (var col in destination.Cols)
            {
                col.Subvector(i, col.Count - i).DivInplace(count - ddof);

            }

            Special.UpperTriangularToHermitianInplace(destination);
        }

        /// <summary>
        /// Computes the mean vector and pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise variance.
        /// </returns>
        public static (Vec<Complex> Mean, Vec<double> Variance) MeanAndVariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var variance = new Vec<double>(mean.Count);
            Variance(xs, mean, variance, ddof);
            return (mean, variance);
        }

        /// <summary>
        /// Computes the mean vector and covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and covariance matrix.
        /// </returns>
        public static (Vec<Complex> Mean, Mat<Complex> Covariance) MeanAndCovariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var mean = xs.Mean();
            var covariance = new Mat<Complex>(mean.Count, mean.Count);
            Covariance(xs, mean, covariance, ddof);
            return (mean, covariance);
        }

        /// <summary>
        /// Computes the mean vector and pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The mean vector and pointwise standard deviation.
        /// </returns>
        public static (Vec<Complex> Mean, Vec<double> StandardDeviation) MeanAndStandardDeviation(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (ddof < 0)
            {
                throw new ArgumentException("The delta degrees of freedom must be a non-negative value.");
            }

            var (mean, tmp) = xs.MeanAndVariance(ddof);
            Debug.Assert(tmp.Stride == 1);
            var span = tmp.Memory.Span;
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Math.Sqrt(span[i]);
            }
            return (mean, tmp);
        }

        /// <summary>
        /// Computes the pointwise variance from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise variance.
        /// </returns>
        public static Vec<double> Variance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndVariance(xs, ddof).Variance;
        }

        /// <summary>
        /// Computes the covariance matrix from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The covariance matrix.
        /// </returns>
        public static Mat<Complex> Covariance(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndCovariance(xs, ddof).Covariance;
        }

        /// <summary>
        /// Computes the pointwise standard deviation from a sequence of vectors.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ddof">
        /// The delta degrees of freedom.
        /// </param>
        /// <returns>
        /// The pointwise standard deviation.
        /// </returns>
        public static Vec<double> StandardDeviation(this IEnumerable<Vec<Complex>> xs, int ddof = 1)
        {
            return MeanAndStandardDeviation(xs, ddof).StandardDeviation;
        }

        private static void AccumulateVariance(in Vec<Complex> x, in Vec<Complex> mean, in Vec<double> destination)
        {
            var sx = x.Memory.Span;
            var sm = mean.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pm = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                var delta = sx[px] - sm[pm];
                sd[pd] += delta.MagnitudeSquared();
                px += x.Stride;
                pm += mean.Stride;
                pd += destination.Stride;
            }
        }

        private static void AccumulateCovariance(in Vec<Complex> centered, in Mat<Complex> destination)
        {
            var sc = centered.Memory.Span;
            var sd = destination.Memory.Span;
            var pc1 = 0;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var pd = destination.Stride * col;
                var pc2 = 0;
                for (var row = 0; row <= col; row++)
                {
                    sd[pd + row] += sc[pc1].Conjugate() * sc[pc2];
                    pc2 += centered.Stride;
                }
                pc1 += centered.Stride;
            }
        }
    }
}
