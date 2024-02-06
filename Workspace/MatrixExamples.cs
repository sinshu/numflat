using System;
using System.Collections.Generic;
using System.Linq;
using NumFlat;

public static class MatrixExamples
{
    public static void Run()
    {
        Example1();
        Example2();
        Example3();
        Example4();
        Example5();
        Example6();
        Example7();
        Example8();
        Example9();
    }

    public static void Example1()
    {
        Console.WriteLine("=== MatrixExample 1 ===");
        Console.WriteLine();

        // The source array.
        var array = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        };

        // Creat a new matrix.
        var matrix = array.ToMatrix();

        // Show the matrix.
        Console.WriteLine(matrix);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example2()
    {
        Console.WriteLine("=== MatrixExample 2 ===");
        Console.WriteLine();

        // Creat a new matrix.
        var matrix = new Mat<double>(3, 3);

        // Element-wise access.
        for (var row = 0; row < matrix.RowCount; row++)
        {
            for (var col = 0; col < matrix.ColCount; col++)
            {
                matrix[row, col] = 10 * row + col;
            }
        }

        // Show the matrix.
        Console.WriteLine(matrix);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example3()
    {
        Console.WriteLine("=== MatrixExample 3 ===");
        Console.WriteLine();

        // Some matrices.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 0, 1, 2 },
            { 0, 0, 1 },
        }
        .ToMatrix();

        var y = new double[,]
        {
            { 1, 0, 0 },
            { 2, 1, 0 },
            { 3, 2, 1 },
        }
        .ToMatrix();

        // Addition.
        var add = x + y;

        // Subtraction.
        var sub = x - y;

        // Multiplication.
        var mul = x * y;

        // Multiplication by a scalar.
        var ms = x * 3;

        // Division by a scalar.
        var ds = x / 3;

        // Pointwise multiplication.
        var pm = x.PointwiseMul(y);

        // Pointwise division.
        var pd = x.PointwiseDiv(y);

        // Transposition.
        var transposed = x.Transpose();

        // Trace.
        var trace = x.Trace();

        // Determinant.
        var determinant = x.Determinant();

        // Rank.
        var rank = x.Rank();

        // Inverse.
        var inverse = x.Inverse();

        // Pseudo-inverse.
        var pseudoInverse = x.PseudoInverse();

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example4()
    {
        Console.WriteLine("=== MatrixExample 4 ===");
        Console.WriteLine();

        // Creat a new matrix.
        var x = new Mat<double>(5, 5);
        x.Fill(3);

        // Create a submatrix of the matrix.
        var sub = x.Submatrix(2, 2, 3, 3);

        // Modify the subvector.
        sub[0, 0] = 100;

        // Show the original matrix.
        Console.WriteLine(x);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example5()
    {
        Console.WriteLine("=== MatrixExample 5 ===");
        Console.WriteLine();

        // Some matrix.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        }
        .ToMatrix();

        // Create a view of a row of the matrix.
        Vec<double> row = x.Rows[0];

        // Create a view of a column of the matrix.
        Vec<double> col = x.Cols[1];

        // Convert a matrix to a row-major jagged array.
        var array = x.Rows.Select(row => row.ToArray()).ToArray();

        // Enumerate all the elements in column-major order.
        var elements = x.Cols.SelectMany(col => col);

        // The mean vector of the row vectors.
        var rowMean = x.Rows.Mean();

        // The covariance matrix of the column vectors.
        var colCov = x.Cols.Covariance();

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example6()
    {
        Console.WriteLine("=== MatrixExample 6 ===");
        Console.WriteLine();

        // Some matrix.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 1, 4, 9 },
            { 1, 3, 7 },
        }
        .ToMatrix();

        // Do LU decomposition.
        var lu = x.Lu();

        // Decomposed matrices.
        var p = lu.GetP();
        var l = lu.L;
        var u = lu.U;

        // Reconstruct the matrix.
        var reconstructed = p * l * u;

        // Show the reconstructed matrix.
        Console.WriteLine(reconstructed);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example7()
    {
        Console.WriteLine("=== MatrixExample 7 ===");
        Console.WriteLine();

        // Some matrix.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        }
        .ToMatrix();

        // Do QR decomposition.
        var qr = x.Qr();

        // Decomposed matrices.
        var q = qr.Q;
        var r = qr.R;

        // Reconstruct the matrix.
        var reconstructed = q * r;

        // Show the reconstructed matrix.
        Console.WriteLine(reconstructed);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example8()
    {
        Console.WriteLine("=== MatrixExample 8 ===");
        Console.WriteLine();

        // Some matrix.
        var x = new double[,]
        {
            { 3, 2, 1 },
            { 2, 3, 2 },
            { 1, 2, 3 },
        }
        .ToMatrix();

        // Do Cholesky decomposition.
        var cholesky = x.Cholesky();

        // Decomposed matrix.
        var l = cholesky.L;

        // Reconstruct the matrix.
        var reconstructed = l * l.Transpose();

        // Show the reconstructed matrix.
        Console.WriteLine(reconstructed);

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void Example9()
    {
        Console.WriteLine("=== MatrixExample 9 ===");
        Console.WriteLine();

        // Some matrix.
        var x = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
        }
        .ToMatrix();

        // Do SVD.
        var svd = x.Svd();

        // Decomposed matrices.
        var s = svd.S.ToDiagonalMatrix();
        var u = svd.U;
        var vt = svd.VT;

        // Reconstruct the matrix.
        var reconstructed = u * s * vt;

        // Show the reconstructed matrix.
        Console.WriteLine(reconstructed);

        Console.WriteLine();
        Console.WriteLine();
    }
}
