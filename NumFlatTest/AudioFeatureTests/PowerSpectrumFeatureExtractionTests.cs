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
        [Test]
        public void ExtensionMethod()
        {
            var fb = new FilterBank(16000, 1024, 100, 7500, 15, FrequencyScale.Linear);

            var x = TestVector.RandomDouble(42, 1024, 3);

            var expected = new Vec<double>(fb.FeatureLength);
            for (var i = 0; i < fb.FeatureLength; i++)
            {
                expected[i] = fb.Filters[i].GetValue(x);
            }

            var actual = fb.Transform(x);

            NumAssert.AreSame(expected, actual, 1.0E-12);
        }
    }
}
