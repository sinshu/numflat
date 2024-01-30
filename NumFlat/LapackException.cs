using System;
using System.Runtime.Serialization;

namespace NumFlat
{
    /// <summary>
    /// Represents errors from LAPACK functions.
    /// </summary>
    public class LapackException : Exception, ISerializable
    {
        private string? functionName = null;
        private int errorCode = 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public LapackException() : base()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public LapackException(string message) : base(message)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public LapackException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LapackException"/>.
        /// </summary>
        public LapackException(string message, string functionName, int errorCode)
            : base($"The function '{functionName}' failed with the error code '{errorCode}'. {message}")
        {
            this.functionName = functionName;
            this.errorCode = errorCode;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected LapackException(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// The name of the LAPACK function.
        /// </summary>
        public string? FunctionName => functionName;

        /// <summary>
        /// The error code returned by the LAPACK function.
        /// </summary>
        public int ErrorCode => errorCode;
    }
}
