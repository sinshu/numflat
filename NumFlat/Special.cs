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
        /// Copies the upper triangular part to the lower triangular part to generate a Hermitian matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        public static void UpperTriangularToHermitianInplace(in Mat<double> x)
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
                    var src = rows[i].Subvector(start, count);
                    var dst = cols[i].Subvector(start, count);
                    src.CopyTo(dst);
                }
            }
        }

        /// <summary>
        /// Copies the upper triangular part to the lower triangular part to generate a Hermitian matrix.
        /// </summary>
        /// <param name="x">
        /// The target matrix.
        /// </param>
        public static void UpperTriangularToHermitianInplace(in Mat<Complex> x)
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
                    var src = rows[i].Subvector(start, count);
                    var dst = cols[i].Subvector(start, count);
                    Vec.Conjugate(src, dst);
                }
            }
        }

        /// <summary>
        /// Calculates the log sum of values in a numerically stable way.
        /// </summary>
        /// <param name="values">
        /// The values to calculate the log sum for.
        /// </param>
        /// <returns>
        /// The log sum of the values.
        /// </returns>
        public static double LogSum(in Vec<double> values)
        {
            ThrowHelper.ThrowIfEmpty(values, nameof(values));

            var max = double.MinValue;
            foreach (var value in values)
            {
                if (value > max)
                {
                    max = value;
                }
            }

            var sum = 0.0;
            foreach (var value in values)
            {
                sum += Math.Exp(value - max);
            }

            return max + Math.Log(sum);
        }

        /// <summary>
        /// Calculates the normalized sinc function.
        /// </summary>
        /// <param name="x">
        /// The input value.
        /// </param>
        /// <returns>
        /// The output value of the sinc function.
        /// </returns>
        public static double Sinc(double x)
        {
            var y = Math.PI * x;
            if (Math.Abs(y) < 1.0E-15)
            {
                return 1.0;
            }
            else
            {
                return Math.Sin(y) / y;
            }
        }

        /// <summary>
        /// Calculates the sigmoid function.
        /// </summary>
        /// <param name="x">
        /// The input value.
        /// </param>
        /// <returns>
        /// The output value of the sigmoid function.
        /// </returns>
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        /// <summary>
        /// Gets <see cref="Memory{T}"/> from the unmanaged pointer.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the values.
        /// </typeparam>
        /// <param name="pointer">
        /// The pointer to the beginning of the unmanaged memory region.
        /// </param>
        /// <param name="length">
        /// The number of elements in the memory region.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Memory{T}"/> that wraps the specified unmanaged memory.
        /// </returns>
        public unsafe static Memory<T> GetMemoryFromUnmanagedPointer<T>(T* pointer, int length) where T : unmanaged
        {
            return new UnmanagedMemoryManager<T>(pointer, length).Memory;
        }

        /// <summary>
        /// Gets <see cref="Memory{T}"/> from the unmanaged pointer.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the values.
        /// </typeparam>
        /// <param name="pointer">
        /// The pointer to the beginning of the unmanaged memory region.
        /// </param>
        /// <param name="length">
        /// The number of elements in the memory region.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Memory{T}"/> that wraps the specified unmanaged memory.
        /// </returns>
        public unsafe static Memory<T> GetMemoryFromUnmanagedPointer<T>(IntPtr pointer, int length) where T : unmanaged
        {
            return new UnmanagedMemoryManager<T>((T*)pointer, length).Memory;
        }
    }
}
