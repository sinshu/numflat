using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class UnsafeIndexerTests
    {
        [Test]
        public void VecIndexer()
        {
            var vec = TestVector.RandomDouble(42, 10, 3);
            var indexer = vec.GetUnsafeFastIndexer();
            for (var i = 0; i < vec.Count; i++)
            {
                Assert.That(vec[i] == indexer[i]);
            }
        }

        [Test]
        public void VecEnumerator()
        {
            var vec = TestVector.RandomDouble(42, 10, 3);
            var i = 0;
            foreach(var value in vec.GetUnsafeFastIndexer())
            {
                Assert.That(vec[i] == value);
                i++;
            }
        }

        [Test]
        public void MatIndexer()
        {
            var mat = TestMatrix.RandomDouble(42, 10, 3, 12);
            var indexer = mat.GetUnsafeFastIndexer();
            for (var row = 0; row < mat.RowCount; row++)
            {
                for (var col = 0; col < mat.ColCount; col++)
                {
                    Assert.That(mat[row, col] == indexer[row, col]);
                }
            }
        }
    }
}
