using System;
using System.Numerics;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public ref struct MemoryChangeDetection<T>
    {
        private Memory<T> target;
        private T[] original;

        public MemoryChangeDetection(Memory<T> target)
        {
            this.target = target;
            this.original = target.ToArray();
        }

        public void Dispose()
        {
            var current = target.ToArray();
            Assert.That(current, Is.EqualTo(original), "Undesired change!");
        }
    }



    public static class MemoryChangeDetection
    {
        public static MemoryChangeDetection<T> EnsureUnchanged<T>(this Vec<T> vector) where T : unmanaged, INumberBase<T>
        {
            return new MemoryChangeDetection<T>(vector.Memory);
        }

        public static MemoryChangeDetection<T> EnsureUnchanged<T>(this Mat<T> matrix) where T : unmanaged, INumberBase<T>
        {
            return new MemoryChangeDetection<T>(matrix.Memory);
        }
    }
}
