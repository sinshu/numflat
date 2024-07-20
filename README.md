# NumFlat

NumFlat is a numerical computation library written entirely in C#.

The goal of this project is to provide a lightweight package for handling various mathematical and computational tasks,
including linear algebra, multivariable analysis, clustering, and signal processing, using only C#.



## Overview

NumFlat provides types named `Vec<T>` and `Mat<T>` for representing vectors and matrices.
These type names are intentionally chosen to avoid confusion with vector and matrix types (like `Vector<T>`) from the `System.Numerics` namespace.
Various linear algebra-related operations can be performed on these types through operator overloading and extension methods.

`Vec<T>` and `Mat<T>` can hold numerical types that implement the `INumberBase<T>` interface, which was newly added in .NET 7.
The primary supported types are `float`, `double`, and `Complex`.
Other types can be used as well, but support beyond simple arithmetic operations is not provided.

```cs
Vec<double> vec = [1, 2, 3];

Mat<double> mat =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

Console.WriteLine(mat * vec);
```



## Installation

.NET 8 is required.

[The NuGet package](https://www.nuget.org/packages/NumFlat) is available.

```ps1
Install-Package NumFlat
```

Most classes are in the `NumFlat` namespace.

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
// Creat a new matrix.
Mat<double> matrix =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

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
Mat<double> x =
[
    [1, 2, 3],
    [0, 1, 2],
    [0, 0, 1],
];
Mat<double> y =
[
    [1, 0, 0],
    [2, 1, 0],
    [3, 2, 1],
];

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
Mat<double> x =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

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
Mat<double> x =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

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

### Solving linear equations
Linear equations like `Ax = b` can be solved with the matrix decomposition methods above. Use `Solve()` method to get the solution vector of a given right-hand side vector `b`.
#### Code
```cs
// Some coefficient matrix.
Mat<double> a =
[
    [1, 2, 3],
    [1, 4, 9],
    [2, 3, 5],
];

// Do LU decomposition.
var lu = a.Lu();

// The right-hand vector.
Vec<double> b = [1, 2, 3];

// Compute the solution vector.
var x = lu.Solve(b);

// Show the solution.
Console.WriteLine(x);
```
#### Output
```console
Vector 3-Double
 2.25
-1.75
 0.75
```

### In-place operations
Most of the operations have an in-place version, which directly modifies the target vector or matrix without creating a new one.
#### Code
```cs
// Some vector.
Vec<double> vector = [1, 2, 3];

// This allocates a new vector.
var normalized = vector.Normalize();

// This modifies the original vector.
vector.NormalizeInplace();

// Some matrix.
Mat<double> matrix =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

// In-place methods can also directly modify a part of vector or matrix.
matrix.Rows[0].NormalizeInplace();
```

### Reducing memory allocation
Most of the operations have a non-allocation version, which requires a destination buffer provided by user. Below is an example of matrix addition without heap allocation.
#### Code
```cs
// Some matrices.
Mat<double> x =
[
    [1, 2],
    [3, 4],
];
Mat<double> y =
[
    [5, 6],
    [7, 8],
];

// This allocates a new matrix.
var answer = x + y;

// The buffer to store the answer.
var destination = new Mat<double>(2, 2);

// This is the same as 'x + y', but does not allocate heap.
Mat.Add(x, y, destination);
```

### Multivariate analyses
The `NumFlat.MultivariateAnalyses` namespace provides functionality related to multivariate analysis.
It currently supports the following methods, with plans to add more methods in the future.
* Principal component analysis (PCA)
* Linear discriminant analysis (LDA)
* Independent component analysis (ICA)
#### Code
```cs
using NumFlat.MultivariateAnalyses;

// Read some data.
IEnumerable<Vec<double>> xs = ReadSomeData();

// Do PCA.
var pca = xs.Pca();

foreach (var x in xs)
{
    // Transform the vector based on the PCA.
    var transformed = pca.Transform(x);
}

// Read some label.
IEnumerable<int> ys = ReadSomeLabel();

// Do LDA.
var lda = xs.Lda(ys);

foreach (var x in xs)
{
    // Transform the vector based on the LDA.
    var transformed = lda.Transform(x);
}
```

### Distributions
The `NumFlat.Distributions` namespace provides functionality related to probability distributions.
It currently supports the multivariate Gaussian distribution and its diagonal covariance matrix variation.
Maximum likelihood estimation from data, probability density function calculation, and random number generation are possible.
#### Code
```cs
using NumFlat.Distributions;

// Read some data.
IEnumerable<Vec<double>> xs = ReadSomeData();

// Compute the maximum likelihood Gaussian distribution.
var gaussian = xs.ToGaussian();

foreach (var x in xs)
{
    // Compute the log PDF.
    var pdf = gaussian.LogPdf(x);
}
```

### Clustering
The `NumFlat.Clustering` namespace provides functionality related to clustering.
It currently supports the following methods, with plans to add more methods in the future.
* k-means
* Gaussian mixture model (GMM)
#### Code
```cs
using NumFlat.Clustering;

// Read some data.
IReadOnlyList<Vec<double>> xs = ReadSomeData();

// Compute a k-means model with 3 clusters.
var kMeans = xs.ToKMeans(3);

// Compute a GMM with 3 clusters.
var gmm = xs.ToGmm(3);
```

### Signal processing
The `NumFlat.SignalProcessing` namespace provides functionality related to signal processing.
It currently supports the following methods, with plans to add more methods in the future.
* Extract frames using the window function
* Overlap addition
* FFT and IFFT
* STFT and ISTFT
* Convolution
* Resampling
#### Code
```cs
using NumFlat.SignalProcessing;

// Some complex vector.
var samples = new Vec<Complex>(256);
samples[0] = 1;

// Do FFT.
var spectrum = samples.Fft();

// Do IFFT.
samples = spectrum.Ifft();
```



## Todo

* ✅ Vector operations
    - ✅ Builder
    - ✅ Indexer
    - ✅ Subvector
    - ✅ Copy, fill, clear
    - ✅ Arithmetic operations
    - ✅ Dot and outer products
    - ✅ Norm and normalization
    - ✅ In-place operations
* ✅ Matrix operations
    - ✅ Builder
    - ✅ Indexer
    - ✅ Submatrix
    - ✅ Copy, fill, clear
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
    - ✅ LU decomposition
    - ✅ QR decomposition
    - ✅ Cholesky decomposition
    - ✅ Singular value decomposition (SVD)
    - ✅ Eigenvalue decomposition (EVD)
    - ✅ Generalized eigenvalue decomposition (GEVD)
* ✅ LINQ-like operations
    - ✅ Sum, mean, variance, covariance for scalars
    - ✅ Sum, mean, variance, covariancee for vectors
    - ✅ Sum, mean, variance for matrices
    - ✅ Weighted sum, mean, variance, covariance for scalars
    - ✅ Weighted sum, mean, variance, covariance for vectors
    - ✅ Weighted sum, mean, variance for matrices
    - ✅ Higher-order statistics
* 🚧 Multivariate analysis
    - ⬜ Linear regression
    - ✅ Principal component analysis (PCA)
    - ✅ Linear discriminant analysis (LDA)
    - ✅ Independent component analysis (ICA)
    - 🚧 Non-negative matrix factorization (NMF)
    - ⬜ Logistic regression
* ⬜ Distributions
    - ✅ Gaussian
    - ✅ Diagonal Gaussian
    - ⬜ Other distributions
* 🚧 Clustering
    - ✅ k-means
    - ✅ Gaussian mixture model (GMM)
    - ⬜ DBSCAN
    - ⬜ OPTICS
* ⬜ Time series
    - ⬜ HMM
* 🚧 Audio signal processing
    - ✅ Fast Fourier transform (FFT)
    - ✅ Short-time Fourier transform (STFT)
    - ✅ Convolution
    - ✅ Resampling
    - 🚧 Feature extraction
    - ⬜ Filtering
* ✅ File IO
    - ✅ CSV
    - ✅ WAV



## License

NumFlat depends on the following libraries.

* [MatFlat](https://github.com/sinshu/matflat) ([MIT license](https://github.com/sinshu/matflat/blob/main/LICENSE.txt))
* [FftFlat](https://github.com/sinshu/fftflat) ([MIT license](https://github.com/sinshu/fftflat/blob/main/LICENSE.md))

NumFlat is available under [the MIT license](LICENSE.txt).
