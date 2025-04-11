using System;
using System.Runtime.Serialization;

namespace EmdFlat
{
    /// <summary>
    /// Represents errors from EmdFlat.
    /// </summary>
    public class EmdException : Exception, ISerializable
    {
        /// <inheritdoc/>
        public EmdException() : base()
        {
        }

        /// <inheritdoc/>
        public EmdException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public EmdException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected EmdException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
