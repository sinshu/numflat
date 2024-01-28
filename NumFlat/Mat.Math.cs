using System;
using System.Numerics;

namespace NumFlat
{
    public static class Mat
    {
        public static void Add<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] + sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void Sub<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] - sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void PointwiseMul<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] * sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }

        public static void PointwiseDiv<T>(Mat<T> x, Mat<T> y, Mat<T> destination) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfEmpty(ref destination, nameof(destination));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y, ref destination);

            var sx = x.Memory.Span;
            var sy = y.Memory.Span;
            var sd = destination.Memory.Span;
            var ox = 0;
            var oy = 0;
            var od = 0;
            while (od < sd.Length)
            {
                var px = ox;
                var py = oy;
                var pd = od;
                var end = od + destination.RowCount;
                while (pd < end)
                {
                    sd[pd] = sx[px] / sy[py];
                    px++;
                    py++;
                    pd++;
                }
                ox += x.Stride;
                oy += y.Stride;
                od += destination.Stride;
            }
        }
    }
}
