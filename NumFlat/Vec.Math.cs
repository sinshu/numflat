using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public static class Vec
    {
        public static void Add<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            if (destination.Count != x.Count || destination.Count != y.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] + sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        public static void Sub<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            if (destination.Count != x.Count || destination.Count != y.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] - sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        public static void PointwiseMul<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            if (destination.Count != x.Count || destination.Count != y.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] * sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }

        public static void PointwiseDiv<T>(Vec<T> x, Vec<T> y, Vec<T> destination) where T : unmanaged, INumberBase<T>
        {
            if (destination.Count != x.Count || destination.Count != y.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var px = 0;
            var py = 0;
            var pd = 0;
            while (pd < sd.Length)
            {
                sd[pd] = sx[px] / sy[py];
                px += x.Stride;
                py += y.Stride;
                pd += destination.Stride;
            }
        }
    }
}
