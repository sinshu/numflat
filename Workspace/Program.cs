using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        VectorExamples.Run();
        MatrixExamples.Run();

        var test = new double[,]
        {
            {  3.0,  0.1, -0.2 },
            {  0.1,  2.5,  0.1 },
            { -0.2,  0.1,  3.1 },
        }
        .ToMatrix();
        Console.WriteLine(test);

        var destination = new CholeskyDouble(test).L;

        Console.WriteLine(test);
        Console.WriteLine(destination);
        Console.WriteLine(destination * destination.Transpose());
    }
}
