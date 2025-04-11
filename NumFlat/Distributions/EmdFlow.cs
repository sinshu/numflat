using System;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Represents a unit of optimal flow between two features in the Earth Mover's Distance (EMD) computation.
    /// </summary>
    public struct EmdFlow
    {
        private int from;
        private int to;
        private double amount;

        internal EmdFlow(int from, int to, double amount)
        {
            this.from = from;
            this.to = to;
            this.amount = amount;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ToString()
        {
            return $"({from}, {to}, {amount})";
        }

        /// <summary>
        /// Gets the index of the source feature in the first signature.
        /// </summary>
        public int From => from;

        /// <summary>
        /// Gets the index of the destination feature in the second signature.
        /// </summary>
        public int To => to;

        /// <summary>
        /// Gets the amount of flow from the source to the destination.
        /// </summary>
        public double Amount => amount;
    }
}
