using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across classification methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the matrix.
    /// </typeparam>
    public interface IClassifier<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Predicts the class of a feature vector.
        /// </summary>
        /// <param name="x">
        /// The feature vector to be classified.
        /// </param>
        /// <returns>
        /// The index of the predicted class.
        /// </returns>
        public int Predict(in Vec<T> x);

        /// <summary>
        /// Gets the required length of a feature vector.
        /// </summary>
        public int Dimension { get; }

        /// <summary>
        /// The number of classes.
        /// </summary>
        public int ClassCount { get; }
    }



    /// <summary>
    /// Provides common functionality across classification methods.
    /// </summary>
    public static class Classifier
    {
        internal static void ThrowIfInvalidSize<T>(IClassifier<T> method, in Vec<T> x, string name) where T : unmanaged, INumberBase<T>
        {
            if (x.Count != method.Dimension)
            {
                throw new ArgumentException($"The classifier requires the length of the vector to be {method.Dimension}, but was {x.Count}.", name);
            }
        }
    }
}
