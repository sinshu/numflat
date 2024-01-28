using System;
using System.Numerics;

namespace NumFlat
{
    public static class MatrixEx
    {
        public static Mat<T> Transpose<T>(this Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            var result = new Mat<T>(x.ColCount, x.RowCount);
            Mat.Transpose(x, result);
            return result;
        }

        public static Mat<double> Inverse(this Mat<double> x)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));

            if (x.RowCount != x.ColCount)
            {
                throw new ArgumentException("`x` must be a square matrix.");
            }

            var result = new Mat<double>(x.RowCount, x.ColCount);
            Mat.Inverse(x, result);
            return result;
        }
    }
}
