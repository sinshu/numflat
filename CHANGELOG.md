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
