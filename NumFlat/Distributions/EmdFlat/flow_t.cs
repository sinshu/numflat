using System;

namespace EmdFlat
{
    /// <summary>
    /// Represents a unit of optimal flow between two features in the Earth Mover's Distance computation.
    /// </summary>
    public struct flow_t
    {
        /// <summary>
        /// Feature number in signature 1.
        /// </summary>
        public int from;

        /// <summary>
        /// Feature number in signature 2.
        /// </summary>
        public int to;

        /// <summary>
        /// Amount of flow from "from" to "to".
        /// </summary>
        public double amount;
    }
}
