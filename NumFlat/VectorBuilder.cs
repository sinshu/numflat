using System;
using System.Numerics;

namespace NumFlat
{
    public static class VectorBuilder
    {
        public static Vec<T> Create<T>(ReadOnlySpan<T> values) where T : unmanaged, INumberBase<T>
        {
            return new Vec<T>(values.ToArray());
        }
    }
}
