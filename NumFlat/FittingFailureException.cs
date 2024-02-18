﻿using System;
using System.Runtime.Serialization;

namespace NumFlat
{
    /// <summary>
    /// Represents errors during a model fitting process.
    /// </summary>
    public class FittingFailureException : NumFlatException, ISerializable
    {
        private string? functionName = null;
        private int errorCode = 0;

        /// <inheritdoc/>
        public FittingFailureException() : base()
        {
        }

        /// <inheritdoc/>
        public FittingFailureException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public FittingFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc/>
        protected FittingFailureException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
