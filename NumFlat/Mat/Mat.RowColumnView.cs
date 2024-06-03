using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        /// <summary>
        /// Gets a view of the matrix as a list of row vectors.
        /// </summary>
        /// <returns>
        /// A view of the matrix as a list of row vectors.
        /// </returns>
        public IEnumerator<Vec<T>> GetEnumerator() => Rows.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Rows.GetEnumerator();



        /// <summary>
        /// Provides a view of the matrix as a list of row vectors.
        /// </summary>
        public struct RowList : IReadOnlyList<Vec<T>>
        {
            private readonly Mat<T> mat;

            internal RowList(in Mat<T> mat)
            {
                this.mat = mat;
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
                    if ((uint)index >= mat.rowCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the number of rows.");
                    }

                    return new Vec<T>(mat.colCount, mat.stride, mat.memory.Slice(index, mat.stride * (mat.colCount - 1) + 1));
                }
            }

            /// <summary>
            /// Gets the number of row vectors.
            /// </summary>
            public int Count => mat.rowCount;

            /// <inheritdoc/>
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
                    this.rowCount = rows.mat.rowCount;
                    this.colCount = rows.mat.colCount;
                    this.stride = rows.mat.stride;
                    this.memory = rows.mat.memory;
                    this.row = -1;
                    this.current = default;
                }

                /// <inheritdoc/>
                public void Dispose()
                {
                }

                /// <inheritdoc/>
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

                /// <inheritdoc/>
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
            private readonly Mat<T> mat;

            internal ColList(in Mat<T> mat)
            {
                this.mat = mat;
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
                    if ((uint)index >= mat.colCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the number of columns.");
                    }

                    return new Vec<T>(mat.rowCount, 1, mat.memory.Slice(mat.stride * index, mat.rowCount));
                }
            }

            /// <summary>
            /// Gets the number of column vectors.
            /// </summary>
            public int Count => mat.colCount;

            /// <inheritdoc/>
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
                    this.rowCount = cols.mat.rowCount;
                    this.stride = cols.mat.stride;
                    this.memory = cols.mat.memory;
                    this.offset = -cols.mat.stride;
                    this.current = default;
                }

                /// <inheritdoc/>
                public void Dispose()
                {
                }

                /// <inheritdoc/>
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

                /// <inheritdoc/>
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
