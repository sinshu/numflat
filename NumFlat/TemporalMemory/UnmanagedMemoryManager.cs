using System;
using System.Buffers;

namespace NumFlat
{
    internal sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
    {
        private readonly T* pointer;
        private readonly int length;

        public UnmanagedMemoryManager(T* pointer, int length)
        {
            this.pointer = pointer;
            this.length = length;
        }

        public override Span<T> GetSpan()
        {
            return new Span<T>(pointer, length);
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            throw new NotSupportedException();
        }

        public override void Unpin()
        {
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
