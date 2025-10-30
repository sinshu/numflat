using System;
using System.Runtime.Serialization;

namespace EmdFlat
{
    /// <summary>
    /// Represents errors from EmdFlat.
    /// </summary>
    public class EmdFlatException : Exception, ISerializable
    {
        /// <inheritdoc/>
        public EmdFlatException() : base()
        {
        }

        /// <inheritdoc/>
        public EmdFlatException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public EmdFlatException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected EmdFlatException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
