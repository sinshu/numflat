using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides LINQ-like operators for matrices and vectors.
    /// </summary>
    public static partial class MathLinq
    {
        private static void AccumulateWeightedSum(in Vec<double> x, double w, in Vec<double> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] += w * sx[px];
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        private static void AccumulateWeightedSum(in Vec<Complex> x, double w, in Vec<Complex> destination)
        {
            var sx = x.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] += w * sx[px];
                px += x.Stride;
                pd += destination.Stride;
            }
        }

        private static void AccumulateWeightedSum(in Mat<double> x, double w, in Mat<double> destination)
        {
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
                    sd[pd] += w * sx[px];
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }

        private static void AccumulateWeightedSum(in Mat<Complex> x, double w, in Mat<Complex> destination)
        {
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
                    sd[pd] += w * sx[px];
                    px++;
                    pd++;
                }
                ox += x.Stride;
                od += destination.Stride;
            }
        }
    }
}
