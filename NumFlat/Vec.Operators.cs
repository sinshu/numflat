using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        public static Vec<T> operator +(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var resut = new Vec<T>(x.count);
            Vec.Add(x, y, resut);
            return resut;
        }

        public static Vec<T> operator -(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            var resut = new Vec<T>(x.count);
            Vec.Sub(x, y, resut);
            return resut;
        }

        public static T operator *(Vec<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(ref x, nameof(x));
            ThrowHelper.ThrowIfEmpty(ref y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(ref x, ref y);

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Vec<T>, Vec<float>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<float>>(ref y);
                var result = Vec.DotProduct(ux, uy);
                return Unsafe.As<float, T>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Vec<T>, Vec<double>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<double>>(ref y);
                var result = Vec.DotProduct(ux, uy);
                return Unsafe.As<double, T>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Vec<T>, Vec<Complex>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<Complex>>(ref y);
                var result = Vec.DotProduct(ux, uy);
                return Unsafe.As<Complex, T>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Dot product for the type `{typeof(T).Name}` is not supported.");
            }
        }
    }
}
