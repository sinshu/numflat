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
        /// An instance of <see cref="RowList.Enumerator"/>.
        /// </returns>
        public RowList.Enumerator GetEnumerator() => Rows.GetEnumerator();

        readonly IEnumerator<Vec<T>> IEnumerable<Vec<T>>.GetEnumerator() => Rows.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => Rows.GetEnumerator();



        /// <summary>
        /// Provides a view of the matrix as a list of row vectors.
        /// </summary>
        public readonly struct RowList : IReadOnlyList<Vec<T>>
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
            public readonly Vec<T> this[int index]
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
            /// Gets a submatrix from the matrix.
            /// </summary>
            /// <param name="range">
            /// The range of the rows.
            /// </param>
            /// <returns>
            /// The specified submatrix in the matrix.
            /// </returns>
            /// <remarks>
            /// This method does not allocate heap memory.
            /// The returned submatrix will be a view of the original matrix.
            /// </remarks>
            public readonly Mat<T> this[Range range]
            {
                get
                {
                    var (offset, length) = range.GetOffsetAndLength(mat.rowCount);
                    return mat.Submatrix(offset, 0, length, mat.colCount);
                }
            }

            /// <summary>
            /// Gets the number of row vectors.
            /// </summary>
            public readonly int Count => mat.rowCount;

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>
            /// An instance of <see cref="Enumerator"/>.
            /// </returns>
            public readonly Enumerator GetEnumerator() => new Enumerator(this);

            readonly IEnumerator<Vec<T>> IEnumerable<Vec<T>>.GetEnumerator() => new Enumerator(this);

            readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);



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

                internal Enumerator(in Mat<T>.RowList rows)
                {
                    this.rowCount = rows.mat.rowCount;
                    this.colCount = rows.mat.colCount;
                    this.stride = rows.mat.stride;
                    this.memory = rows.mat.memory;
                    this.row = -1;
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /// <inheritdoc/>
                public Vec<T> Current => new Vec<T>(colCount, stride, memory.Slice(row, stride * (colCount - 1) + 1));

                object? IEnumerator.Current => new Vec<T>(colCount, stride, memory.Slice(row, stride * (colCount - 1) + 1));

                void IEnumerator.Reset()
                {
                    this.row = -1;
                }
            }
        }



        /// <summary>
        /// Provides a view of the matrix as a list of column vectors.
        /// </summary>
        public readonly struct ColList : IReadOnlyList<Vec<T>>
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
            public readonly Vec<T> this[int index]
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
            /// Gets a submatrix from the matrix.
            /// </summary>
            /// <param name="range">
            /// The range of the columns.
            /// </param>
            /// <returns>
            /// The specified submatrix in the matrix.
            /// </returns>
            /// <remarks>
            /// This method does not allocate heap memory.
            /// The returned submatrix will be a view of the original matrix.
            /// </remarks>
            public readonly Mat<T> this[Range range]
            {
                get
                {
                    var (offset, length) = range.GetOffsetAndLength(mat.colCount);
                    return mat.Submatrix(0, offset, mat.rowCount, length);
                }
            }

            /// <summary>
            /// Gets the number of column vectors.
            /// </summary>
            public readonly int Count => mat.colCount;

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>
            /// An instance of <see cref="Enumerator"/>.
            /// </returns>
            public readonly Enumerator GetEnumerator() => new Enumerator(this);

            readonly IEnumerator<Vec<T>> IEnumerable<Vec<T>>.GetEnumerator() => new Enumerator(this);

            readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);



            /// <summary>
            /// Enumerates the column vectors.
            /// </summary>
            public struct Enumerator : IEnumerator<Vec<T>>, IEnumerator
            {
                private readonly int rowCount;
                private readonly int stride;
                private readonly Memory<T> memory;
                private int offset;

                internal Enumerator(in Mat<T>.ColList cols)
                {
                    this.rowCount = cols.mat.rowCount;
                    this.stride = cols.mat.stride;
                    this.memory = cols.mat.memory;
                    this.offset = -cols.mat.stride;
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /// <inheritdoc/>
                public Vec<T> Current => new Vec<T>(memory.Slice(offset, rowCount));

                object? IEnumerator.Current => new Vec<T>(memory.Slice(offset, rowCount));

                void IEnumerator.Reset()
                {
                    this.offset = -stride;
                }
            }
        }
    }
}
