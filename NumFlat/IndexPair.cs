using System;

namespace NumFlat
{
    /// <summary>
    /// Represents a pair of indices.
    /// </summary>
    public struct IndexPair
    {
        private int first;
        private int second;

        internal IndexPair(int first, int second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString()
        {
            return "(" + first + ", " + second + ")";
        }

        /// <summary>
        /// The first index.
        /// </summary>
        public int First => first;

        /// <summary>
        /// The second index.
        /// </summary>
        public int Second => second;
    }
}
