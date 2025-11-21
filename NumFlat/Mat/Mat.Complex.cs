using System;
using System.Numerics;

namespace NumFlat
{
    public static partial class Mat
    {
        /// <summary>
        /// Conjugates the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be conjugated.
        /// </param>
        /// <param name="destination">
        /// The destination of the complex matrix conjugation.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// To efficiently perform matrix multiplication with matrix conjugation,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void Conjugate(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(x, destination);

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Conjugate();
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the Hermitian transpose of a complex matrix, X^H.
        /// </summary>
        /// <param name="x">
        /// The complex matrix to be transposed.
        /// </param>
        /// <param name="destination">
        /// The destination of the Hermitian transposition.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// Since in-place transposition is not supported,
        /// <paramref name="x"/> and <paramref name="destination"/> must be different.
        /// To efficiently perform matrix multiplication with matrix transposition,
        /// use <see cref="Mat.Mul(in Mat{Complex}, in Mat{Complex}, in Mat{Complex}, bool, bool, bool, bool)"/>.
        /// </remarks>
        public static void ConjugateTranspose(in Mat<Complex> x, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.ColCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.ColCount'.");
            }

            if (destination.ColCount != x.RowCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.RowCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            for (var col = 0; col < destination.ColCount; col++)
            {
                var px = col;
                var pd = destination.Stride * col;
                var end = pd + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Conjugate();
                    px += x.Stride;
                    pd++;
                }
            }
        }

        private static void ConjugateDiv(in Vec<Complex> x, double y, in Vec<Complex> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px].Conjugate() / y;
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        /// <summary>
        /// Extracts the real part of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <param name="destination">
        /// The destination of the real parts.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Real(in Mat<Complex> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Real;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Extracts the imaginary part of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <param name="destination">
        /// The destination of the imaginary parts.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Imaginary(in Mat<Complex> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Imaginary;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the magnitude of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <param name="destination">
        /// The destination of the magnitudes.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Magnitude(in Mat<Complex> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Magnitude;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the squared magnitude of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <param name="destination">
        /// The destination of the squared magnitudes.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void MagnitudeSquared(in Mat<Complex> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    var value = sx[px];
                    sd[pd] = value.Real * value.Real + value.Imaginary * value.Imaginary;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        /// <summary>
        /// Computes the phase of each element in the complex matrix.
        /// </summary>
        /// <param name="x">
        /// The complex matrix.
        /// </param>
        /// <param name="destination">
        /// The destination of the phases.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void Phase(in Mat<Complex> x, in Mat<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (destination.RowCount != x.RowCount)
            {
                throw new ArgumentException("'destination.RowCount' must match 'x.RowCount'.");
            }

            if (destination.ColCount != x.ColCount)
            {
                throw new ArgumentException("'destination.ColCount' must match 'x.ColCount'.");
            }

            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px].Phase;
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }
    }
}
