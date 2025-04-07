# v1.0.4

* Added `Special.GetMemoryFromUnmanagedPointer` to support matrices backed by unmanaged memory.
* Added `DynamicTimeWarping`.

# v1.0.3

* Indexers for `Vec<T>` and `Mat<T>` now return `ref T`, improving usability.

# v1.0.2

* Revised the complex linear regression formula from `y = w^H * x + b` to `y = w^T * x + b`.

# v1.0.1

* Some code optimizations.
* Added complex linear regression.

# v1.0.0

* Improved DBSCAN implementation.
* Added linear regression.

# v0.10.8

* Added DBSCAN.
* Added `GetFrameTime` method to `StftInfo`.
* Added `Copy` extension method to `Vec<T>` and `Mat<T>`.
* Updated MatFlat to 0.8.2.

# v0.10.7

* Added logistic regression.

# v0.10.6

* Added support for `Range`.

# v0.10.5

* Added real FFT.

# v0.10.4

* Added NMF.

# v0.10.3

* The length of the result of the `Resample()` extension method now matches MATLAB's implementation.
* Fixed a potential issue where temporary matrix allocation, which is internally used in the library, might fail.
* Added multiplicative update implementation for NMF.

# v0.10.2

* Fixed issue where resampling failed on long signal.

# v0.10.1

* Added `MultivariateDistribution.Generate()` method without args which uses `Random.Shared`.
* Added resampling.

# v0.10.0

* Audio feature extraction supports complex spectrum.
* Added random sampling method for distributions.
* Some code cleanup.

# v0.9.7

* Added options for clustering algorithms.
* Added options for ICA.
* Some code cleanup.

# v0.9.6

* Fixed bug where SVD does not return if the matrix contains NaN values.
* Added ICA.
* Optimized vector value enumeration.
* Some code cleanup.

# v0.9.5

* Improved `Convolve` method performance for short signals.
* `CsvFile` now supports writing `Vec<T>` to a file.
* Avoid memory allocation in enumeration.
* Some code cleanup.

# v0.9.4

* Matrices can now be created using collection expressions.

# v0.9.3

* Added convolution.
* Added (weighted) sum for scalars, vectors, and matrices.

# v0.9.2

* Added filter bank audio feature extraction.

# v0.9.1

* Removed unnecessary code that handle native dependencies.
* `Determinant()` of matrix decomposition object for non-square matrices now throws an exception.

# v0.9.0

* Dropped native dependencies.

# v0.8.1

* Added `NumFlat.IO.CsvFile` class for CSV file IO.

# v0.8.0

* Added `NumFlat.IO` namespace for file IO.
* Added `NumFlat.IO.WaveFile` class for wave file IO.

# v0.7.7

* Optimized k-means and GMM by omitting some calculations.
* GMM now behaves the same as sklearn's default GMM.

# v0.7.6

* Added STFT and ISTFT.
* Added `WindowFunctions` class.

# v0.7.5

* Moved `FourierTransform` class to `SignalProcessing` namespace.
* Added the methods for framing and overlap-add.
* Optimized Bhattacharyya distance.
* Now `UnsafeFastIndexer` for vectors can be used with `foreach`.

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
