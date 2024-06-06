using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.IO;

namespace NumFlatTest.IOTests
{
    public class CsvFileTests
    {
        [Test]
        public void Read()
        {
            var expected = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
                { 10, 11, 12 },
            }
            .ToMatrix();

            var actual = CsvFile.Read(Path.Combine("dataset", "simple.csv"), 1);

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void WriteMat()
        {
            var expected = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 },
                { 10, 11, 12 },
            }
            .ToMatrix();

            CsvFile.Write("test.csv", expected);

            var actual = CsvFile.Read("test.csv");

            NumAssert.AreSame(expected, actual, 0);
        }

        [Test]
        public void WriteVec()
        {
            var expected = Enumerable.Range(0, 10).Select(i => (double)i).ToVector();

            CsvFile.Write("test.csv", expected);

            var actual = CsvFile.Read("test.csv");
            Assert.That(actual.ColCount == 1);

            NumAssert.AreSame(expected, actual.Cols[0], 0);
        }
    }
}
