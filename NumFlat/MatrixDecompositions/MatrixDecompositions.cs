using System;
using System.Numerics;
using MatFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides extension methods for matrix decomposition.
    /// </summary>
    public static class MatrixDecompositions
    {
        /// <summary>
        /// Computes the eigenvalue decomposition (EVD).
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EigenValueDecompositionSingle"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static EigenValueDecompositionSingle Evd(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new EigenValueDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the eigenvalue decomposition (EVD).
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EigenValueDecompositionDouble"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static EigenValueDecompositionDouble Evd(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new EigenValueDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the eigenvalue decomposition (EVD).
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EigenValueDecompositionComplex"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the EVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static EigenValueDecompositionComplex Evd(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new EigenValueDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="LuDecompositionSingle"/>.
        /// </returns>
        public static LuDecompositionSingle Lu(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="LuDecompositionDouble"/>.
        /// </returns>
        public static LuDecompositionDouble Lu(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the LU decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="LuDecompositionComplex"/>.
        /// </returns>
        public static LuDecompositionComplex Lu(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new LuDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="QrDecompositionSingle"/>.
        /// </returns>
        public static QrDecompositionSingle Qr(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="QrDecompositionDouble"/>.
        /// </returns>
        public static QrDecompositionDouble Qr(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the QR decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="QrDecompositionComplex"/>.
        /// </returns>
        public static QrDecompositionComplex Qr(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new QrDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CholeskyDecompositionSingle"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static CholeskyDecompositionSingle Cholesky(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CholeskyDecompositionDouble"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static CholeskyDecompositionDouble Cholesky(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the Cholesky decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of <see cref="CholeskyDecompositionComplex"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// The matrix is ill-conditioned.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static CholeskyDecompositionComplex Cholesky(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new CholeskyDecompositionComplex(a);
        }

        /// <summary>
        /// Gets the singular values from the matrix.
        /// </summary>
        /// <param name="a">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The singular values.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static Vec<float> GetSingularValues(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<float>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionSingle.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Gets the singular values from the matrix.
        /// </summary>
        /// <param name="a">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The singular values.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static Vec<double> GetSingularValues(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionDouble.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Gets the singular values from the matrix.
        /// </summary>
        /// <param name="a">
        /// The target matrix.
        /// </param>
        /// <returns>
        /// The singular values.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static Vec<double> GetSingularValues(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            var s = new Vec<double>(Math.Min(a.RowCount, a.ColCount));
            SingularValueDecompositionComplex.GetSingularValues(a, s);
            return s;
        }

        /// <summary>
        /// Computes the singular value decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionSingle"/>'.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static SingularValueDecompositionSingle Svd(in this Mat<float> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionSingle(a);
        }

        /// <summary>
        /// Computes the singular value decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionDouble"/>'.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static SingularValueDecompositionDouble Svd(in this Mat<double> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionDouble(a);
        }

        /// <summary>
        /// Computes the singular value decomposition.
        /// </summary>
        /// <param name="a">
        /// The matrix to be decomposed.
        /// </param>
        /// <returns>
        /// An instance of '<see cref="SingularValueDecompositionComplex"/>'.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the SVD.
        /// </exception>
        public static SingularValueDecompositionComplex Svd(in this Mat<Complex> a)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));

            return new SingularValueDecompositionComplex(a);
        }

        /// <summary>
        /// Computes the generalized eigenvalue decomposition (GEVD).
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <returns>
        /// An instance of <see cref="GeneralizedEigenValueDecompositionSingle"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static GeneralizedEigenValueDecompositionSingle Gevd(in this Mat<float> a, in Mat<float> b)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            return new GeneralizedEigenValueDecompositionSingle(a, b);
        }

        /// <summary>
        /// Computes the generalized eigenvalue decomposition (GEVD).
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <returns>
        /// An instance of <see cref="GeneralizedEigenValueDecompositionDouble"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static GeneralizedEigenValueDecompositionDouble Gevd(in this Mat<double> a, in Mat<double> b)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            return new GeneralizedEigenValueDecompositionDouble(a, b);
        }

        /// <summary>
        /// Computes the generalized eigenvalue decomposition (GEVD).
        /// </summary>
        /// <param name="a">
        /// The source matrix A.
        /// </param>
        /// <param name="b">
        /// The source matrix B.
        /// </param>
        /// <returns>
        /// An instance of <see cref="GeneralizedEigenValueDecompositionComplex"/>.
        /// </returns>
        /// <exception cref="MatrixFactorizationException">
        /// Failed to compute the GEVD.
        /// </exception>
        /// <remarks>
        /// The matrix to be decomposed must be symmetric positive definite.
        /// Note that this implementation does not verify whether the input matrix is symmetric.
        /// Specifically, only the upper triangular part of the input matrix is used, and the rest is ignored.
        /// </remarks>
        public static GeneralizedEigenValueDecompositionComplex Gevd(in this Mat<Complex> a, in Mat<Complex> b)
        {
            ThrowHelper.ThrowIfEmpty(a, nameof(a));
            ThrowHelper.ThrowIfEmpty(b, nameof(b));

            return new GeneralizedEigenValueDecompositionComplex(a, b);
        }
    }
}
