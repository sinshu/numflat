using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        public static Vec<T> operator +(Vec<T> x, Vec<T> y)
        {
            if (x.count != y.count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var destination = new Vec<T>(x.count);
            Vec.Add(x, y, destination);
            return destination;
        }

        public static Vec<T> operator -(Vec<T> x, Vec<T> y)
        {
            if (x.count != y.count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            var destination = new Vec<T>(x.count);
            Vec.Sub(x, y, destination);
            return destination;
        }
    }
}
