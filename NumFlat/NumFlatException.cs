using System;
using System.Runtime.Serialization;

namespace NumFlat
{
    /// <summary>
    /// Represents errors from NumFlat.
    /// </summary>
    public class NumFlatException : Exception, ISerializable
    {
        /// <inheritdoc/>
        public NumFlatException() : base()
        {
        }

        /// <inheritdoc/>
        public NumFlatException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public NumFlatException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected NumFlatException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
