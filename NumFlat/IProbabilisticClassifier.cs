using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides common functionality across probabilistic classification methods.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in vectors.
    /// </typeparam>
    public interface IProbabilisticClassifier<T> : IClassifier<T> where T : unmanaged, INumberBase<T>
    {
        /// <summary>
        /// Predicts the class of a feature vector.
        /// </summary>
        /// <param name="x">
        /// The feature vector to be classified.
        /// </param>
        /// <param name="destination">
        /// The destination of the probabilities for each class.
        /// </param>
        public void PredictProbability(in Vec<T> x, in Vec<T> destination);
    }



    /// <summary>
    /// Provides common functionality across probabilistic classification methods.
    /// </summary>
    public static class ProbabilisticClassifier
    {
        internal static void ThrowIfInvalidSize<T>(IProbabilisticClassifier<T> method, in Vec<T> source, in Vec<T> destination, string sourceName, string destinationName) where T : unmanaged, INumberBase<T>
        {
            if (source.Count != method.Dimension)
            {
                throw new ArgumentException($"The classifier requires the length of the source vector to be {method.Dimension}, but was {source.Count}.", sourceName);
            }

            if (destination.Count != method.ClassCount)
            {
                throw new ArgumentException($"The classifier requires the length of the destination vector to be {method.ClassCount}, but was {destination.Count}.", destinationName);
            }
        }

        /// <summary>
        /// Predicts the class of a feature vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="method">
        /// The classification method.
        /// </param>
        /// <param name="x">
        /// The feature vector to be classified.
        /// </param>
        /// <returns>
        /// The probabilities for each class.
        /// </returns>
        public static Vec<T> PredictProbability<T>(this IProbabilisticClassifier<T> method, in Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            Classifier.ThrowIfInvalidSize(method, x, nameof(x));

            var destination = new Vec<T>(method.ClassCount);
            method.PredictProbability(x, destination);
            return destination;
        }
    }
}
