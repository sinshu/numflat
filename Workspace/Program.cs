using System;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        var x = new Vec<double>(new double[] { 1, 2, 3 });
        var y = new Vec<double>(new double[] { 4, 5, 6 });
        var result = x * y;
        Console.WriteLine(result);
    }
}
