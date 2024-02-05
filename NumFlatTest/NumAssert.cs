using System;
using System.Linq;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class NumAssert
    {
        public static void AreSame(Mat<double> x, Mat<double> y, double delta)
        {
            var xValues = x.Cols.SelectMany(col => col);
            var yValues = y.Cols.SelectMany(col => col);
            var errors = xValues.Zip(yValues, (xValue, yValue) => Math.Abs(xValue - yValue));

            if (errors.Max() > delta)
            {
                Assert.Fail();
            }
        }
    }
}
