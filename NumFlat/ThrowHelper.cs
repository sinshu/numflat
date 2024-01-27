using System;
using System.Numerics;
using System.Xml.Linq;

namespace NumFlat
{
    internal static class ThrowHelper
    {
        internal static void ThrowIfEmpty<T>(ref Vec<T> vector, string name) where T : struct, INumberBase<T>
        {
            if (vector.Count == 0)
            {
                throw new ArgumentException("The vector must not be empty.", name);
            }
        }

        internal static void ThrowIfDifferentLength<T>(ref Vec<T> vector1, ref Vec<T> vector2) where T : struct, INumberBase<T>
        {
            if (vector1.Count != vector2.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }
        }

        internal static void ThrowIfDifferentLength<T>(ref Vec<T> vector1, ref Vec<T> vector2, ref Vec<T> vector3) where T : struct, INumberBase<T>
        {
            if (vector1.Count != vector2.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }

            if (vector1.Count != vector3.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }
        }
    }
}
