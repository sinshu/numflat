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

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Add(x, y, result);
            return result;
        }

        public static Mat<T> operator -(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Sub(x, y, result);
            return result;
        }

        public static Vec<T> operator *(Mat<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));

            if (y.Count != x.ColCount)
            {
                throw new ArgumentException("`y.Count` must match `x.ColCount`.");
            }

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Mat<T>, Mat<float>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<float>>(ref y);
                var result = new Vec<float>(x.rowCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Vec<float>, Vec<T>>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Mat<T>, Mat<double>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<double>>(ref y);
                var result = new Vec<double>(x.rowCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Vec<double>, Vec<T>>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Mat<T>, Mat<Complex>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<Complex>>(ref y);
                var result = new Vec<Complex>(x.rowCount);
                Mat.Mul(ux, uy, result);
                return Unsafe.As<Vec<Complex>, Vec<T>>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Matrix multiplication for the type `{typeof(T).Name}` is not supported.");
            }
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
