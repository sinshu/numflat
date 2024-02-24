using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat.Distributions;

namespace NumFlat.Clustering
{
    public class GaussianMixtureModel : IProbabilisticClassifier<double>
    {
        private readonly Component[] components;

        public GaussianMixtureModel(IEnumerable<Component> components)
        {
            this.components = components.ToArray();
        }

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

            public Component(double weight, Gaussian gaussian)
            {
                this.weight = weight;
                this.gaussian = gaussian;
            }
        }
    }
}
