using System;
using System.Numerics;

namespace NumFlat
{
    internal static class UnsafeFastIndexer
    {
        internal static Vec<T>.UnsafeFastIndexer GetUnsafeFastIndexer<T>(in this Vec<T> source) where T : unmanaged, INumberBase<T>
        {
            return new Vec<T>.UnsafeFastIndexer(source);
        }

        internal static Mat<T>.UnsafeFastIndexer GetUnsafeFastIndexer<T>(in this Mat<T> source) where T : unmanaged, INumberBase<T>
        {
            return new Mat<T>.UnsafeFastIndexer(source);
        }
    }
}
