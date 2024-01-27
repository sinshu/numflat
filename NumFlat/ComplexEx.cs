using System;
using System.Numerics;

namespace NumFlat
{
    public static class ComplexEx
    {
        public static Complex Conjugate(this Complex value)
        {
            return new Complex(value.Real, -value.Imaginary);
        }
    }
}
