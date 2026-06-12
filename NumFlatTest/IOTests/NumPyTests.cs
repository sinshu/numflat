using System;
using System.IO;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.IO;

namespace NumFlatTest.IOTests
{
    public class NumPyTests
    {
        [Test]
        public void ReadVectorFloat()
        {
            var expected = new[] { 1.5f, -2.25f, 3.125f, 4.5f }.ToVector();

            var actual = NumPy.ReadVector<float>(Path.Combine("dataset", "numpy_vector_float32.npy"));

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void ReadVectorDouble()
        {
            var expected = new[] { 1.25, -2.5, 3.75, 4.125 }.ToVector();

            var actual = NumPy.ReadVector<double>(Path.Combine("dataset", "numpy_vector_float64.npy"));

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void ReadVectorComplex()
        {
            var expected = new[]
            {
                new Complex(1, 2),
                new Complex(-3.5, 4.25),
                new Complex(0, -1.5),
            }
            .ToVector();

            var actual = NumPy.ReadVector<Complex>(Path.Combine("dataset", "numpy_vector_complex128.npy"));

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void ReadMatrixDouble()
        {
            var expected = new double[,]
            {
                { 1.0, 2.0, 3.0 },
                { 4.5, -5.25, 6.125 },
            }
            .ToMatrix();

            var actual = NumPy.ReadMatrix<double>(Path.Combine("dataset", "numpy_matrix_float64.npy"));

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void ReadMatrixComplex()
        {
            var expected = new Complex[,]
            {
                { new Complex(1, 2), new Complex(3, -4) },
                { new Complex(-5.5, 6.25), new Complex(7.75, -8.5) },
            }
            .ToMatrix();

            var actual = NumPy.ReadMatrix<Complex>(Path.Combine("dataset", "numpy_matrix_complex128.npy"));

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void WriteVector()
        {
            var expected = new[] { 1.25, -2.5, 3.75, 4.125 }.ToVector();

            NumPy.WriteVector(expected, "test_vector.npy");
            var actual = NumPy.ReadVector<double>("test_vector.npy");

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void WriteMatrix()
        {
            var expected = new Complex[,]
            {
                { new Complex(1, 2), new Complex(3, -4) },
                { new Complex(-5.5, 6.25), new Complex(7.75, -8.5) },
            }
            .ToMatrix();

            NumPy.WriteMatrix(expected, "test_matrix.npy");
            var actual = NumPy.ReadMatrix<Complex>("test_matrix.npy");

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void ReadMatrixRejectsVector()
        {
            Action action = () => { _ = NumPy.ReadMatrix<double>(Path.Combine("dataset", "numpy_vector_float64.npy")); };
            Assert.Throws<InvalidDataException>(action);
        }
    }
}
