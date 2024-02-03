# NumFlat

The purpose of this project is to provide a convenient C# wrapper for [OpenBLAS](https://github.com/OpenMathLib/OpenBLAS). It aims to allow numerical computations related to linear algebra to be performed naturally in C# code.



## Installation

To be prepared 😖



## Overview

NumFlat provides types named `Vec<T>` and `Mat<T>` for representing vectors and matrices. These type names are intentionally chosen to avoid confusion with vector and matrix types from the `System.Numerics` namespace.

`Vec<T>` and `Mat<T>` can hold numerical types that implement the `INumberBase<T>` interface. The primary supported types are `float`, `double`, and `Complex`. Other types can be used as well, but support beyond simple arithmetic operations is not provided.



## Usage

### Create a new vector

A new vector can be created by listing elements inside `[]`.

#### Code
```cs
// Creat a new vector.
Vec<double> vector = [1, 2, 3];

// Show the vector.
Console.WriteLine(vector);
```
#### Output
```console
Vector 3-Double
1
2
3
```

### Create a new vector from `IEnumerable<T>`

Vectors can also be created from objects that implement `IEnumerable<T>`.

#### Code
```cs
// Some enumerable.
var enumerable = Enumerable.Range(0, 10).Select(i => i / 10.0);

// Create a vector from an enumerable.
var vector = enumerable.ToVector();

// Show the vector.
Console.WriteLine(vector);
```
#### Output
```console
Vector 10-Double
  0
0.1
0.2
0.3
0.4
0.5
0.6
0.7
0.8
0.9
```

### Indexer access for vectors

Elements in a vector can be accessed or modified through the indexer.

#### Code
```cs
// Creat a new vector.
var vector = new Vec<double>(3);

// Element-wise access.
vector[0] = 4;
vector[1] = 5;
vector[2] = 6;

// Show the vector.
Console.WriteLine(vector);
```
#### Output
```console
Vector 3-Double
4
5
6
```

### Vector arithmetic

Basic operations on vectors are provided through operator overloading and extension methods.

#### Code
```cs
// Some vectors.
Vec<double> x = [1, 2, 3];
Vec<double> y = [4, 5, 6];

// Addition.
var add = x + y;

// Subtraction.
var sub = x - y;

// Multiplication by a scalar.
var ms = x * 3;

// Division by a scalar.
var ds = x / 3;

// Pointwise multiplication.
var pm = x.PointwiseMul(y);

// Pointwise division.
var pd = x.PointwiseDiv(y);

// Dot product.
var dot = x * y;

// Outer product.
var outer = x.Outer(y);
```

### Subvector

A subvector can be created from a vector. The subvector acts as a view of the original vector, and changes to the subvector will affect the original vector.

#### Code
```cs
// Some vector.
Vec<double> x = [3, 3, 3, 3, 3];

// Create a subvector of the vector.
var sub = x.Subvector(2, 3);

// Modify the subvector.
sub[0] = 100;

// Show the original vector.
Console.WriteLine(x);
```
#### Output
```console
Vector 5-Double
  3
  3
100
  3
  3
```

### Creating matrix

Matrices can be generated from 2D arrays.

#### Code
```cs
// Creat a new matrix.
var array = new double[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 },
};

var matrix = array.ToMatrix();

// Show the matrix.
Console.WriteLine(matrix);
```
#### Output
```console
Matrix 3x3-Double
1  2  3
4  5  6
7  8  9
```

### Indexer access for matrices

Elements in a matrix can be accessed or modified through the indexer.

#### Code
```cs
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
```
#### Output
```console
Matrix 3x3-Double
 0   1   2
10  11  12
20  21  22
```

### Matrix arithmetic

Basic operations on matrices are provided through operator overloading and extension methods.

#### Code
```cs
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
```

### Submatrix

A submatrix can be created from a matrix. The submatrix acts as a view of the original matrix, and changes to the submatrix will affect the original matrix.

#### Code
```cs
// Creat a new matrix.
var x = new Mat<double>(5, 5);
x.Fill(3);

// Create a submatrix of the matrix.
var sub = x.Submatrix(2, 2, 3, 3);

// Modify the subvector.
sub[0, 0] = 100;

// Show the original matrix.
Console.WriteLine(x);
```
#### Output
```console
Matrix 5x5-Double
3  3    3  3  3
3  3    3  3  3
3  3  100  3  3
3  3    3  3  3
3  3    3  3  3
```

### Treat matrix as a set of vectors

Views of rows or columns as vectors can be obtained through the `Rows` or `Cols` properties. Similar to a submatrix, changes to the view will affect the original matrix.

#### Code
```cs
// Some matrix.
var x = new double[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 },
}
.ToMatrix();

// Create a view of a row of the matrix.
Vec<double> row = x.Rows[1];

// Create a view of a column of the matrix.
Vec<double> col = x.Cols[2];

// The mean vector of the row vectors.
var rowMean = x.Rows.Mean();

// The covariance matrix of the column vectors.
var colCov = x.Cols.Covariance();

// Enumerate all the values in column-major order.
var values = x.Cols.SelectMany(col => col);
```

### Singular value decomposition

The singular value decomposition can be obtained by calling the extension method `Svd()`.

#### Code
```cs
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
var s = svd.S;
var u = svd.U;
var vt = svd.VT;

// Reconstruct the matrix.
var reconstructed = u * s.ToDiagonalMatrix() * vt;

// Show the reconstructed matrix.
Console.WriteLine(reconstructed);
```
#### Output
```console
Matrix 3x3-Double
1  2  3
4  5  6
7  8  9
```
