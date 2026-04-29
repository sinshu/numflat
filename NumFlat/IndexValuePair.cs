using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumFlat
{
    /// <summary>
    /// Represents a pair of index and value.
    /// </summary>
    public struct IndexValuePair<T>
    {
        private int index;
        private T value;

        internal IndexValuePair(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString()
        {
            return "(" + index + ", " + value + ")";
        }

        /// <summary>
        /// The index.
        /// </summary>
        public int Index => index;

        /// <summary>
        /// The value.
        /// </summary>
        public T Value => value;
    }
}
