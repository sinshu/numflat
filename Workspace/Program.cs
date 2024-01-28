using System;
using System.Numerics;
using NumFlat;

public class Program
{
    public static void Main(string[] args)
    {
        var x = new Vec<Complex>(new Complex[] { 1, 2, 3 });
        var y = new Vec<Complex>(new Complex[] { 4, 5, 6 });
        x *= 0.5;
        y *= 3;
        var result = x * y;
        Console.WriteLine(result);
    }
}
