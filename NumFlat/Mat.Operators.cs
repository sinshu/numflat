using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        public static Mat<T> operator +(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var destination = new Mat<T>(x.rowCount, x.colCount);
            Mat.Add(x, y, destination);
            return destination;
        }

        public static Mat<T> operator -(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var destination = new Mat<T>(x.rowCount, x.colCount);
            Mat.Sub(x, y, destination);
            return destination;
        }

        public static Mat<T> operator *(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            if (x.colCount != y.rowCount)
            {
                throw new ArgumentException("`y.RowCount` must match `x.ColCount`.");
            }

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Mat<T>, Mat<float>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<float>>(ref y);
                var result = new Mat<float>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Mat<float>, Mat<T>>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Mat<T>, Mat<double>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<double>>(ref y);
                var result = new Mat<double>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Mat<double>, Mat<T>>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Mat<T>, Mat<Complex>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<Complex>>(ref y);
                var result = new Mat<Complex>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Mat<Complex>, Mat<T>>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Matrix multiplication for the type `{typeof(T).Name}` is not supported.");
            }
        }
    }
}
