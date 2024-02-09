using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        /// <summary>
        /// Provides a view of the matrix as a list of row vectors.
        /// </summary>
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

            /// <summary>
            /// Gets the row vector at the specified row index.
            /// </summary>
            /// <param name="index">
            /// The row index.
            /// </param>
            /// <returns>
            /// The row vector.
            /// </returns>
            public Vec<T> this[int index]
            {
                get
                {
                    if ((uint)index >= rowCount)
                    {
                        throw new IndexOutOfRangeException("'index' must be within 'Mat<T>.RowCount'.");
                    }

                    return new Vec<T>(colCount, stride, memory.Slice(index, stride * (colCount - 1) + 1));
                }
            }

            /// <summary>
            /// Gets the number of row vectors.
            /// </summary>
            public int Count => rowCount;

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public IEnumerator<Vec<T>> GetEnumerator() => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Vec<T>>)this).GetEnumerator();



            /// <summary>
            /// Enumerates the row vectors.
            /// </summary>
            public struct Enumerator : IEnumerator<Vec<T>>, IEnumerator
            {
                private readonly int rowCount;
                private readonly int colCount;
                private readonly int stride;
                private readonly Memory<T> memory;
                private int row;
                private Vec<T> current;

                internal Enumerator(in Mat<T>.RowList rows)
                {
                    this.rowCount = rows.rowCount;
                    this.colCount = rows.colCount;
                    this.stride = rows.stride;
                    this.memory = rows.memory;
                    this.row = -1;
                    this.current = default;
                }

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
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

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
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



        /// <summary>
        /// Provides a view of the matrix as a list of column vectors.
        /// </summary>
        public struct ColList : IReadOnlyList<Vec<T>>
        {
            private readonly int rowCount;
            private readonly int colCount;
            private readonly int stride;
            private readonly Memory<T> memory;

            internal ColList(in Mat<T> mat)
            {
                this.rowCount = mat.rowCount;
                this.colCount = mat.colCount;
                this.stride = mat.stride;
                this.memory = mat.memory;
            }

            /// <summary>
            /// Gets the column vector at the specified column index.
            /// </summary>
            /// <param name="index">
            /// The column index.
            /// </param>
            /// <returns>
            /// The column vector.
            /// </returns>
            public Vec<T> this[int index]
            {
                get
                {
                    if ((uint)index >= colCount)
                    {
                        throw new IndexOutOfRangeException("'index' must be within 'Mat<T>.ColCount'.");
                    }

                    return new Vec<T>(rowCount, 1, memory.Slice(stride * index, rowCount));
                }
            }

            /// <summary>
            /// Gets the number of column vectors.
            /// </summary>
            public int Count => colCount;

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public IEnumerator<Vec<T>> GetEnumerator() => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Vec<T>>)this).GetEnumerator();



            /// <summary>
            /// Enumerates the column vectors.
            /// </summary>
            public struct Enumerator : IEnumerator<Vec<T>>, IEnumerator
            {
                private readonly int rowCount;
                private readonly int stride;
                private readonly Memory<T> memory;
                private int offset;
                private Vec<T> current;

                internal Enumerator(in Mat<T>.ColList cols)
                {
                    this.rowCount = cols.rowCount;
                    this.stride = cols.stride;
                    this.memory = cols.memory;
                    this.offset = -cols.stride;
                    this.current = default;
                }

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
                public bool MoveNext()
                {
                    offset += stride;
                    if (offset < memory.Length)
                    {
                        current = new Vec<T>(memory.Slice(offset, rowCount));
                        return true;
                    }
                    else
                    {
                        current = default;
                        return false;
                    }
                }

                /// <summary>
                /// <inheritdoc/>
                /// </summary>
                public Vec<T> Current => current;

                object? IEnumerator.Current
                {
                    get => current;
                }

                void IEnumerator.Reset()
                {
                    this.offset = -stride;
                    this.current = default;
                }
            }
        }
    }
}
