using OpenBlasSharp;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace NumFlat
{
    public static class MathLinq
    {
        public static void Mean(this IEnumerable<Vec<double>> xs, Vec<double> destination)
        {
            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                Vec.Add(destination, x, destination);
                count++;
            }

            Vec.Div(destination, count, destination);
        }

        public static unsafe void Covariance(this IEnumerable<Vec<double>> xs, Vec<double> mean, Mat<double> destination, int ddot)
        {
            destination.Clear();

            var count = 0;
            var buffer = ArrayPool<double>.Shared.Rent(mean.Count);
            try
            {
                Memory<double> memory = buffer;
                var centered = new Vec<double>(memory.Slice(0, mean.Count));
                fixed (double* pd = destination.Memory.Span)
                fixed (double* pc = centered.Memory.Span)
                {
                    foreach (var x in xs)
                    {
                        Vec.Sub(x, mean, centered);
                        Blas.Dger(
                            Order.ColMajor,
                            destination.RowCount, destination.ColCount,
                            1.0,
                            pc, centered.Stride,
                            pc, centered.Stride,
                            pd, destination.Stride);
                        count++;
                    }
                }
            }
            finally
            {
                ArrayPool<double>.Shared.Return(buffer);
            }

            Mat.Mul(destination, 1.0 / (count - ddot), destination);
        }
    }
}
