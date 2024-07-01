using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.AudioFeatures;

namespace NumFlatTest.AufioFeatureTests
{
    public class PowerSpectrumFeatureExtractionTests
    {
        [TestCase(1)]
        [TestCase(3)]
        public void ExtensionMethod(int xStride)
        {
            var fb = new FilterBank(16000, 1024, 100, 7500, 15, FrequencyScale.Linear);

            var x = TestVector.RandomDouble(42, 1024, xStride);

            var expected = new Vec<double>(fb.FeatureLength);
            for (var i = 0; i < fb.FeatureLength; i++)
            {
                expected[i] = fb.Filters[i].GetValue(x);
            }

            var actual = fb.Transform(x);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }

        [TestCase(1)]
        [TestCase(3)]
        public void ExtensionMethod_Complex(int xStride)
        {
            var fb = new FilterBank(16000, 1024, 100, 7500, 15, FrequencyScale.Linear);

            var x = TestVector.RandomComplex(42, 1024, xStride);

            var expected = new Vec<double>(fb.FeatureLength);
            var power = x.Map(NumFlat.ComplexExtensions.MagnitudeSquared);
            for (var i = 0; i < fb.FeatureLength; i++)
            {
                expected[i] = fb.Filters[i].GetValue(power);
            }

            var actual = fb.Transform(x);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
