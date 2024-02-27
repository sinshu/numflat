# v0.7.4

* Now `MathLinq` supports weighted second order statistics for matrices.
* Added the following extension methods.
     - `double Skewness(this IEnumerable<double> xs, bool unbiased = true)`
     - `double Kurtosis(this IEnumerable<double> xs, bool unbiased = true)`

# v0.7.3

* Now `MathLinq` supports weighted second order statistics for scalars (`double` and `Complex`).

# v0.7.2

* Now `MathLinq` supports second order statistics for scalars (`double` and `Complex`).

# v0.7.1

* Added GMM.

# v0.7.0

* Now `Gaussian` has a constructor that requires `IEnumerable<Vec<double>>`.
* Added another overload of `Svd.Decompose()` that computes only `S` and `U`.
* Now PCA uses SVD instead of EVD.
* Added k-means clustering.

# v0.6.2

* Added `MapInplace()` to `Vec<T>` and `Mat<T>`.
* Added `Bhattacharyya()` to `Gaussian`.
* Added `GetUnsafeFastIndexer()` to `Vec<T>` and `Mat<T>`.

# v0.6.1

* Added `FittingFailureException` to indicate model fitting failure.
* Added `regularization` parameter to `ToGaussian()`.
* Added pointwise scalar addition and subtraction for vectors and matrices.
* Added `Distance()` to `Vec<T>`.
* Added `Mahalanobis()` to `Gaussian`.

# v0.6.0

* Added the Gaussian and diagonal Gaussian distributions.
* Optimized the Cholesky decomposition implementation.

# v0.5.0

* Added FFT.

# v0.4.0

* Improved error handling and error messages.
* Added `Mat<T>.InverseInplace()`.
* Added PCA and LDA.

# v0.3.2

* Added the following in-place operations.
    - `Vec<T>.ReverseInplace()`
    - `Vec<Complex>.ConjugateInplace()`
    - `Mat<T>.TransposeInplace()`
    - `Mat<Complex>.ConjugateInplace()`
    - `Mat<Complex>.ConjugateTransposeInplace()`
* Matrix decomposition methods now support `Solve()` against matrices as a set of RHS vectors.

# v0.3.1

* Added the weighted version of `Mean`, `Variance`, `Covariance`, `StandardDeviation` for vectors.

# v0.3.0

* Added the `Determinant` and `LogDeterminant` methods to matrix decomposition objects.
* Now `MathLinq` supports `Mean` and `Variance` for matrices.
* Optimized `Covariance` by utilizing the symmetry of matrices.
* Added the following norm-related methods to `Vec<T>`.
    - `Norm()`
    - `Norm(p)`
    - `L1Norm()`
    - `InfinityNorm()`
    - `Normalize()`
    - `Normalize(p)`
* Added the following norm-related methods to `Mat<T>`.
    - `FrobeniusNorm()`
    - `L1Norm()`
    - `L2Norm()`
    - `InfinityNorm()`
* Added in-place operations for vectors and matrices.  
In-place operations have a method name with the suffix `Inplace` (`NormalizeInplace` for example).

# v0.2.2

* Added the following builder methods.
    - `Vec<T> VectorBuilder.Fill<T>(int count, T value)`
    - `Vec<T> VectorBuilder.FromFunc<T>(int count, Func<int, T> func)`
    - `Mat<T> MatrixBuilder.Fill<T>(int rowCount, int colCount, T value)`
    - `Mat<T> MatrixBuilder.FromFunc<T>(int rowCount, int colCount, Func<int, int, T> func)`
* Added the `Variance` method to `MathLinq`.
* Added the generalized eigen value decomposition.

# v0.2.1

* Revised the doc comments and error messages.
* Added the eigen value decomposition.

# v0.2.0

* Revised the unit tests for better robustness.
* Revised the doc comments and error messages.
* Now the LU decomposition directly exposes L and U like the other decomposition methods.

# v0.1.1

* Revised the readme included in the NuGet package.

# v0.1.0

* This is the first release.
