using System;
using System.Collections;
using System.Collections.Generic;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        /// <summary>
        /// Provides a view of the matrix as a list of diagonal elements.
        /// </summary>
        public ref struct DiagonalElements
        {
            private readonly ref readonly Mat<T> mat;

            internal DiagonalElements(in Mat<T> mat)
            {
                this.mat = ref mat;
            }

            /// <summary>
            /// Gets an enumerator.
            /// </summary>
            /// <returns>
            /// An instance of <see cref="DiagonalElementsEnumerator"/>.
            /// </returns>
            public DiagonalElementsEnumerator GetEnumerator()
            {
                return new DiagonalElementsEnumerator(in mat);
            }



            /// <summary>
            /// Enumerates the diagonal elements.
            /// </summary>
            public ref struct DiagonalElementsEnumerator
            {
                private readonly Span<T> span;
                private readonly int step;
                private int position;

                internal DiagonalElementsEnumerator(in Mat<T> mat)
                {
                    step = mat.stride + 1;
                    span = mat.Memory.Span.Slice(0, (Math.Min(mat.rowCount, mat.colCount) - 1) * step + 1);
                    position = -step;
                }

                /// <summary>
                /// Stops the enumerator.
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// Advances the enumerator.
                /// </summary>
                /// <returns>
                /// True if the enumerator has the next value.
                /// </returns>
                public bool MoveNext()
                {
                    position += step;
                    if (position < span.Length)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /// <summary>
                /// Gets the current value.
                /// </summary>
                public ref T Current => ref span[position];
            }
        }
    }
}
