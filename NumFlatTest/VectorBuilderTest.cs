using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public class VectorBuilderTest
    {
        [Test]
        public void Create()
        {
            var source = new int[] { 1, 2, 3 };
            var vector = VectorBuilder.Create<int>(source);
            CollectionAssert.AreEqual(source, vector.ToArray());
        }
    }
}
