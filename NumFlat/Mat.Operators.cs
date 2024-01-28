using System;
using System.Numerics;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        public static Mat<T> operator +(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var destination = new Mat<T>(x.RowCount, x.ColCount);
            Mat.Add(x, y, destination);
            return destination;
        }

        public static Mat<T> operator -(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var destination = new Mat<T>(x.RowCount, x.ColCount);
            Mat.Sub(x, y, destination);
            return destination;
        }
    }
}
