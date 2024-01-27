using System;
using System.Numerics;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        public static Vec<T> operator +(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentLength(ref x, ref y);

            var destination = new Vec<T>(x.count);
            Vec.Add(x, y, destination);
            return destination;
        }

        public static Vec<T> operator -(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentLength(ref x, ref y);

            var destination = new Vec<T>(x.count);
            Vec.Sub(x, y, destination);
            return destination;
        }
    }
}
