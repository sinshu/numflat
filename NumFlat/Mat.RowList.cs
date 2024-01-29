using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        public struct RowList : IReadOnlyList<Vec<T>>
        {
            private readonly int rowCount;
            private readonly int colCount;
            private readonly int stride;
            private readonly Memory<T> memory;

            internal RowList(in Mat<T> mat)
            {
                this.rowCount = mat.rowCount;
                this.colCount = mat.colCount;
                this.stride = mat.stride;
                this.memory = mat.memory;
            }

            public Vec<T> this[int index]
            {
                get
                {
                    if ((uint)index >= rowCount)
                    {
                        throw new IndexOutOfRangeException("`index` must be within `Mat<T>.RowCount`.");
                    }

                    return new Vec<T>(colCount, stride, memory.Slice(index, stride * (colCount - 1) + 1));
                }
            }

            public int Count => rowCount;

            public IEnumerator<Vec<T>> GetEnumerator() => new Enumerator(ref this);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Vec<T>>)this).GetEnumerator();



            public struct Enumerator : IEnumerator<Vec<T>>, IEnumerator
            {
                private readonly int rowCount;
                private readonly int colCount;
                private readonly int stride;
                private readonly Memory<T> memory;
                private int row;
                private Vec<T> current;

                internal Enumerator(ref Mat<T>.RowList rows)
                {
                    this.rowCount = rows.rowCount;
                    this.colCount = rows.colCount;
                    this.stride = rows.stride;
                    this.memory = rows.memory;
                    this.row = -1;
                    this.current = default;
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    row++;
                    if (row < rowCount)
                    {
                        current = new Vec<T>(colCount, stride, memory.Slice(row, stride * (colCount - 1) + 1));
                        return true;
                    }
                    else
                    {
                        current = default;
                        return false;
                    }
                }

                public Vec<T> Current => current;

                object? IEnumerator.Current
                {
                    get => current;
                }

                void IEnumerator.Reset()
                {
                    this.row = -1;
                    this.current = default;
                }
            }
        }
    }
}
