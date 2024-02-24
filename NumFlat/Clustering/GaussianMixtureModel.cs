using System;
using NumFlat.Distributions;

namespace NumFlat.Clustering
{
    public class GaussianMixtureModel : IProbabilisticClassifier<double>
    {
        public int Dimension => throw new NotImplementedException();

        public int ClassCount => throw new NotImplementedException();

        public int Predict(in Vec<double> x)
        {
            throw new NotImplementedException();
        }

        public void PredictProbability(in Vec<double> x, in Vec<double> destination)
        {
            throw new NotImplementedException();
        }

        public struct Component
        {
            private double weight;
            private Gaussian gaussian;
        }
    }
}
