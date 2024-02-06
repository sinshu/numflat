﻿using System;
using System.Buffers;
using System.Numerics;

namespace NumFlat
{
    internal ref struct TemporalMatrix<T> where T : unmanaged, INumberBase<T>
    {
        private IMemoryOwner<T>? owner;
        public readonly Mat<T> Item;

        public TemporalMatrix(int rowCount, int colCount)
        {
            var length = rowCount * colCount;
            owner = MemoryPool<T>.Shared.Rent(length);
            Item = new Mat<T>(rowCount, colCount, rowCount, owner.Memory.Slice(0, length));
        }

        public void Dispose()
        {
            if (owner != null)
            {
                owner.Dispose();
                owner = null;
            }
        }
    }



    internal static class TemporalMatrix
    {
        public static TemporalMatrix<T> CopyFrom<T>(in Mat<T> source) where T : unmanaged, INumberBase<T>
        {
            var tmp = new TemporalMatrix<T>(source.RowCount, source.ColCount);
            source.CopyTo(tmp.Item);
            return tmp;
        }
    }
}
