using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class Interop
    {
        public static MathNet.Numerics.LinearAlgebra.Vector<float> ToMathNet(Vec<float> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Single.DenseVector(x.Count);
            for (var i = 0; i < x.Count; i++)
            {
                mathNet[i] = x[i];
            }
            return mathNet;
        }

        public static MathNet.Numerics.LinearAlgebra.Matrix<float> ToMathNet(Mat<float> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Single.DenseMatrix(x.RowCount, x.ColCount);
            for (var col = 0; col < x.ColCount; col++)
            {
                for (var row = 0; row < x.RowCount; row++)
                {
                    mathNet[row, col] = x[row, col];
                }
            }
            return mathNet;
        }

        public static MathNet.Numerics.LinearAlgebra.Vector<double> ToMathNet(Vec<double> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(x.Count);
            for (var i = 0; i < x.Count; i++)
            {
                mathNet[i] = x[i];
            }
            return mathNet;
        }

        public static MathNet.Numerics.LinearAlgebra.Matrix<double> ToMathNet(Mat<double> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(x.RowCount, x.ColCount);
            for (var col = 0; col < x.ColCount; col++)
            {
                for (var row = 0; row < x.RowCount; row++)
                {
                    mathNet[row, col] = x[row, col];
                }
            }
            return mathNet;
        }

        public static MathNet.Numerics.LinearAlgebra.Vector<Complex> ToMathNet(Vec<Complex> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Complex.DenseVector(x.Count);
            for (var i = 0; i < x.Count; i++)
            {
                mathNet[i] = x[i];
            }
            return mathNet;
        }

        public static MathNet.Numerics.LinearAlgebra.Matrix<Complex> ToMathNet(Mat<Complex> x)
        {
            var mathNet = new MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix(x.RowCount, x.ColCount);
            for (var col = 0; col < x.ColCount; col++)
            {
                for (var row = 0; row < x.RowCount; row++)
                {
                    mathNet[row, col] = x[row, col];
                }
            }
            return mathNet;
        }
    }
}
