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
