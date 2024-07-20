using System;
using System.Runtime.Serialization;

namespace NumFlat
{
    /// <summary>
    /// Represents errors that occur in matrix factorization.
    /// </summary>
    public class MatrixFactorizationException : NumFlatException, ISerializable
    {
        /// <inheritdoc/>
        public MatrixFactorizationException() : base()
        {
        }

        /// <inheritdoc/>
        public MatrixFactorizationException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public MatrixFactorizationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected MatrixFactorizationException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
