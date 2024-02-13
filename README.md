# NumFlat

NumFlat is a numerical computation library for C#.
The goal of this project is to create an easy-to-use C# wrapper for [OpenBLAS](https://github.com/OpenMathLib/OpenBLAS).
It aims to enable writing numerical computation processes related to linear algebra in natural C# code.



## Overview

NumFlat provides types named `Vec<T>` and `Mat<T>` for representing vectors and matrices.
These type names are intentionally chosen to avoid confusion with vector and matrix types (like `Vector<T>`) from the `System.Numerics` namespace.
Various linear algebra-related operations can be performed on these types through operator overloading and extension methods.

`Vec<T>` and `Mat<T>` can hold numerical types that implement the `INumberBase<T>` interface, which was newly added in .NET 7.
The primary supported types are `float`, `double`, and `Complex`.
Other types can be used as well, but support beyond simple arithmetic operations is not provided.



## Installation

.NET 8 is required.

[The NuGet package](https://www.nuget.org/packages/NumFlat) is available.

```ps1
Install-Package NumFlat
```

This package does not include the native binary for OpenBLAS.
[The `OpenBlasSharp.Windows` package](https://www.nuget.org/packages/OpenBlasSharp.Windows) provides the native binary for Windows.

```ps1
Install-Package OpenBlasSharp.Windows
```

Or, [download the compiled binary](https://github.com/OpenMathLib/OpenBLAS/releases) and put `libopenblas.dll` in the same directory as the executable file.
Binaries for both x86 and x64 architectures are supported, but [the ILP64 build with the `x64-64` suffix](https://github.com/OpenMathLib/OpenBLAS/blob/develop/docs/distributing.md#ilp64-interface-builds) is not supported.

All the classes are in the `NumFlat` namespace.

```cs
using NumFlat;
```



## Usage

### Create a new vector

A new vector can be created by listing elements inside `[]`.

#### Code
```cs
// Create a new vector.
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
Since the vector itself is an `IEnumerable<T>`, it is also possible to call LINQ methods on the vector if needed.

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
// Create a new vector.
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

// L2 norm.
var l2Norm = x.Norm();

// L1 norm.
var l1Norm = x.L1Norm();

// Infinity norm.
var infinityNorm = x.InfinityNorm();

// Normalization.
var normalized = x.Normalize();
```

### Subvector

A subvector can be created from a vector.
The subvector acts as a view of the original vector, and changes to the subvector will affect the original vector.

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

// L1 norm.
var l1Norm = x.L1Norm();

// L2 norm.
var l2Norm = x.L2Norm();

// Infinity norm.
var infinityNorm = x.InfinityNorm();
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

### Matrix as a set of vectors

Views of rows or columns as vectors can be obtained through the `Rows` or `Cols` properties.
Similar to a submatrix, changes to the view will affect the original matrix.
These properties implement `IEnumerable<Vec<T>>`, allowing for LINQ methods to be called on collections of vectors.

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
```

### Matrix decomposition

The LU, QR, Cholesky, SVD, EVD, and GEVD for all three major number types `float`, `double`, and `Complex` are available. Below is an example to decompose a matrix using SVD.

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
var decomposition = x.Svd();

// Decomposed matrices.
var s = decomposition.S.ToDiagonalMatrix();
var u = decomposition.U;
var vt = decomposition.VT;

// Reconstruct the matrix.
var reconstructed = u * s * vt;

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

### In-place operations

Most of the operations have an in-place version, which directly modifies the target vector or matrix without creating a new one.

```cs
// Some vector.
Vec<double> vector = [1, 2, 3];

// This allocates a new vector.
var normalized = vector.Normalize();

// This modifies the original vector.
vector.NormalizeInplace();

// Some matrix.
var matrix = new double[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 },
}
.ToMatrix();

// In-place methods can also directly modify a part of vector or matrix.
matrix.Rows[0].NormalizeInplace();
```

### Reducing memory allocation

Most of the operations have a non-allocation version, which requires a destination buffer provided by user. Below is an example of matrix addition without heap allocation.

```cs
// Some matrices.
var x = new double[,]
{
    { 1, 2 },
    { 3, 4 } ,
}
.ToMatrix();

var y = new double[,]
{
    { 5, 6 },
    { 7, 8 } ,
}
.ToMatrix();

// This allocates a new matrix.
var answer = x + y;

// The buffer to store the answer.
var destination = new Mat<double>(2, 2);

// This is the same as 'x + y', but does not allocate heap.
Mat.Add(x, y, destination);
```



## Todo

* ✅ OpenBLAS wrapper (see [OpenBlasSharp](https://github.com/sinshu/OpenBlasSharp))
* ✅ Vector operations
    - ✅ Builder
    - ✅ Indexer
    - ✅ Subvector
    - ✅ Copy, Fill, Clear
    - ✅ Arithmetic operations
    - ✅ Dot and Outer products
    - ✅ Norm and normalization
    - ✅ In-place operations
* ✅ Matrix operations
    - ✅ Builder
    - ✅ Indexer
    - ✅ Submatrix
    - ✅ Copy, Fill, Clear
    - ✅ Arithmetic operations
    - ✅ Transposition
    - ✅ Trace
    - ✅ Determinant
    - ✅ Rank
    - ✅ Inversion
    - ✅ Pseudo-inverse
    - ✅ Norm
    - ✅ In-place operations
* ✅ Matrix Decomposition
    - ✅ LU
    - ✅ QR
    - ✅ Cholesky
    - ✅ SVD
    - ✅ EVD
    - ✅ GEVD
* ⬜ LINQ-like operations
    - ⬜ Mean, Variance, Covariance for scalars
    - ✅ Mean, Variance, Covariance for vectors
    - ✅ Mean, Variance for matrices
    - ⬜ Weighted Mean, Variance, Covariance for scalars
    - ✅ Weighted Mean, Variance, Covariance for vectors
    - ⬜ Weighted Mean, Variance for matrices
    - ⬜ Higher-order statistics
* ⬜ Multivariate analysis
    - ⬜ Linear regression
    - ⬜ PCA
    - ⬜ LDA
    - ⬜ ICA
    - ⬜ NMF
* ⬜ Distributions
    - ⬜ Gaussian
* ⬜ Clustering
    - ⬜ k-means
    - ⬜ GMM
* ⬜ Time series
    - ⬜ HMM
* ⬜ DSP
    - ⬜ FFT
    - ⬜ Filtering



## License

NumFlat depends on the following libraries.

* [OpenBLAS](https://github.com/OpenMathLib/OpenBLAS) ([BSD-3-Clause license](https://github.com/OpenMathLib/OpenBLAS/blob/develop/LICENSE))
* [OpenBlasSharp](https://github.com/sinshu/OpenBlasSharp) ([BSD-3-Clause license](https://github.com/sinshu/OpenBlasSharp/blob/main/LICENSE.txt))

NumFlat is available under [MIT license](LICENSE.txt).
