using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;

namespace NumFlatTest
{
    public static class Utility
    {
        public static float NextSingle(Random random)
        {
            return 2 * random.NextSingle() - 1;
        }

        public static float NextSingleNonZero(Random random)
        {
            var value = 2 * random.NextSingle() - 1;
            if (value >= 0)
            {
                return value + 1;
            }
            else
            {
                return value - 1;
            }
        }

        public static double NextDouble(Random random)
        {
            return 2 * random.NextDouble() - 1;
        }

        public static double NextDoubleNonZero(Random random)
        {
            var value = 2 * random.NextDouble() - 1;
            if (value >= 0)
            {
                return value + 1;
            }
            else
            {
                return value - 1;
            }
        }

        public static Complex NextComplex(Random random)
        {
            return new Complex(NextDouble(random), NextDouble(random));
        }

        public static Complex NextComplexNonZero(Random random)
        {
            return new Complex(NextDoubleNonZero(random), NextDoubleNonZero(random));
        }
    }
}
