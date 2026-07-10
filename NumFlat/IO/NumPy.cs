using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NumFlat.IO
{
    /// <summary>
    /// Provides NumPy .npy file IO.
    /// </summary>
    public static class NumPy
    {
        private static readonly byte[] Magic = { 0x93, (byte)'N', (byte)'U', (byte)'M', (byte)'P', (byte)'Y' };

        /// <summary>
        /// Reads a .npy file as a <see cref="Vec{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="path">
        /// The path of the .npy file.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Vec{T}"/>.
        /// </returns>
        public static Vec<T> ReadVector<T>(string path) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var stream = File.OpenRead(path))
            {
                return ReadVector<T>(stream);
            }
        }

        /// <summary>
        /// Reads a .npy stream as a <see cref="Vec{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="stream">
        /// The stream that contains .npy data.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Vec{T}"/>.
        /// </returns>
        public static Vec<T> ReadVector<T>(Stream stream) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(stream, nameof(stream));
            ThrowIfNotLittleEndian();

            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            var header = ReadHeader(reader);
            if (header.Shape.Length != 1)
            {
                throw new InvalidDataException("The .npy file does not contain a one-dimensional array.");
            }

            var dtype = GetDtype<T>();
            if (header.Descriptor != dtype)
            {
                throw new InvalidDataException($"The .npy dtype '{header.Descriptor}' does not match '{dtype}'.");
            }

            var vector = new Vec<T>(header.Shape[0]);
            var fv = vector.GetUnsafeFastIndexer();
            for (var i = 0; i < vector.Count; i++)
            {
                fv[i] = ReadValue<T>(reader);
            }
            return vector;
        }

        /// <summary>
        /// Reads a .npy file as a <see cref="Mat{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="path">
        /// The path of the .npy file.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Mat{T}"/>.
        /// </returns>
        public static Mat<T> ReadMatrix<T>(string path) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var stream = File.OpenRead(path))
            {
                return ReadMatrix<T>(stream);
            }
        }

        /// <summary>
        /// Reads a .npy stream as a <see cref="Mat{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="stream">
        /// The stream that contains .npy data.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Mat{T}"/>.
        /// </returns>
        public static Mat<T> ReadMatrix<T>(Stream stream) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfNull(stream, nameof(stream));
            ThrowIfNotLittleEndian();

            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            var header = ReadHeader(reader);
            if (header.Shape.Length != 2)
            {
                throw new InvalidDataException("The .npy file does not contain a two-dimensional array.");
            }

            var dtype = GetDtype<T>();
            if (header.Descriptor != dtype)
            {
                throw new InvalidDataException($"The .npy dtype '{header.Descriptor}' does not match '{dtype}'.");
            }

            var matrix = new Mat<T>(header.Shape[0], header.Shape[1]);
            var fm = matrix.GetUnsafeFastIndexer();
            for (var row = 0; row < matrix.RowCount; row++)
            {
                for (var col = 0; col < matrix.ColCount; col++)
                {
                    fm[row, col] = ReadValue<T>(reader);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Writes a <see cref="Vec{T}"/> as a .npy file.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="vector">
        /// The <see cref="Vec{T}"/> to be written.
        /// </param>
        /// <param name="path">
        /// The path of the .npy file.
        /// </param>
        public static void WriteVector<T>(in Vec<T> vector, string path) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(vector, nameof(vector));
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var stream = File.Create(path))
            {
                WriteVector(vector, stream);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vec{T}"/> as a .npy stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="vector">
        /// The <see cref="Vec{T}"/> to be written.
        /// </param>
        /// <param name="stream">
        /// The stream to which .npy data is written.
        /// </param>
        public static void WriteVector<T>(in Vec<T> vector, Stream stream) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(vector, nameof(vector));
            ThrowHelper.ThrowIfNull(stream, nameof(stream));
            ThrowIfNotLittleEndian();

            using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);

            WriteHeader(writer, GetDtype<T>(), vector.Count);
            var fv = vector.GetUnsafeFastIndexer();
            for (var i = 0; i < vector.Count; i++)
            {
                WriteValue(writer, fv[i]);
            }
        }

        /// <summary>
        /// Writes a <see cref="Mat{T}"/> as a .npy file.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="matrix">
        /// The <see cref="Mat{T}"/> to be written.
        /// </param>
        /// <param name="path">
        /// The path of the .npy file.
        /// </param>
        public static void WriteMatrix<T>(in Mat<T> matrix, string path) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(matrix, nameof(matrix));
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var stream = File.Create(path))
            {
                WriteMatrix(matrix, stream);
            }
        }

        /// <summary>
        /// Writes a <see cref="Mat{T}"/> as a .npy stream.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="matrix">
        /// The <see cref="Mat{T}"/> to be written.
        /// </param>
        /// <param name="stream">
        /// The stream to which .npy data is written.
        /// </param>
        public static void WriteMatrix<T>(in Mat<T> matrix, Stream stream) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(matrix, nameof(matrix));
            ThrowHelper.ThrowIfNull(stream, nameof(stream));
            ThrowIfNotLittleEndian();

            using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);

            WriteHeader(writer, GetDtype<T>(), matrix.RowCount, matrix.ColCount);
            var fm = matrix.GetUnsafeFastIndexer();
            for (var row = 0; row < matrix.RowCount; row++)
            {
                for (var col = 0; col < matrix.ColCount; col++)
                {
                    WriteValue(writer, fm[row, col]);
                }
            }
        }

        private static void ThrowIfNotLittleEndian()
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new PlatformNotSupportedException("NumPy .npy IO supports only little-endian platforms.");
            }
        }

        private static NpyHeader ReadHeader(BinaryReader reader)
        {
            var magic = new byte[Magic.Length];
            reader.BaseStream.ReadExactly(magic);
            for (var i = 0; i < Magic.Length; i++)
            {
                if (magic[i] != Magic[i])
                {
                    throw new InvalidDataException("The stream does not contain a NumPy .npy file.");
                }
            }

            var major = reader.ReadByte();
            var minor = reader.ReadByte();
            var headerLength = major switch
            {
                1 => reader.ReadUInt16(),
                2 => reader.ReadInt32(),
                _ => throw new NotSupportedException($"NumPy .npy version {major}.{minor} is not supported."),
            };

            var headerBytes = new byte[headerLength];
            reader.BaseStream.ReadExactly(headerBytes);
            var header = Encoding.ASCII.GetString(headerBytes);
            return ParseHeader(header);
        }

        private static NpyHeader ParseHeader(string header)
        {
            var descriptorMatch = Regex.Match(header, "'descr'\\s*:\\s*'(?<descr>[^']+)'");
            if (!descriptorMatch.Success)
            {
                throw new InvalidDataException("The .npy header does not contain 'descr'.");
            }

            if (!Regex.IsMatch(header, "'fortran_order'\\s*:\\s*False"))
            {
                throw new NotSupportedException("The .npy array must use C-order storage.");
            }

            var shapeMatch = Regex.Match(header, "'shape'\\s*:\\s*\\((?<shape>[^)]*)\\)");
            if (!shapeMatch.Success)
            {
                throw new InvalidDataException("The .npy header does not contain 'shape'.");
            }

            var parts = shapeMatch.Groups["shape"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                throw new NotSupportedException("Scalar .npy arrays are not supported.");
            }

            var shape = new int[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                shape[i] = int.Parse(parts[i], CultureInfo.InvariantCulture);
                if (shape[i] <= 0)
                {
                    throw new NotSupportedException("The .npy array cannot be empty.");
                }
            }

            return new NpyHeader(descriptorMatch.Groups["descr"].Value, shape);
        }

        private static void WriteHeader(BinaryWriter writer, string dtype, params int[] shape)
        {
            writer.Write(Magic);
            writer.Write((byte)1);
            writer.Write((byte)0);

            var shapeText = shape.Length == 1 ? $"{shape[0]}," : string.Join(", ", shape);
            var header = $"{{'descr': '{dtype}', 'fortran_order': False, 'shape': ({shapeText}), }}";
            var padding = 16 - ((Magic.Length + 2 + 2 + header.Length + 1) % 16);
            if (padding == 16)
            {
                padding = 0;
            }
            header = header + new string(' ', padding) + "\n";

            var headerBytes = Encoding.ASCII.GetBytes(header);
            if (headerBytes.Length > ushort.MaxValue)
            {
                throw new NotSupportedException("The .npy header is too large for version 1.0.");
            }

            writer.Write((ushort)headerBytes.Length);
            writer.Write(headerBytes);
        }

        private static string GetDtype<T>() where T : unmanaged, INumberBase<T>
        {
            if (typeof(T) == typeof(float))
            {
                return "<f4";
            }

            if (typeof(T) == typeof(double))
            {
                return "<f8";
            }

            if (typeof(T) == typeof(Complex))
            {
                return "<c16";
            }

            throw new NotSupportedException("The element type must be float, double, or Complex.");
        }

        private static T ReadValue<T>(BinaryReader reader) where T : unmanaged, INumberBase<T>
        {
            if (typeof(T) == typeof(float))
            {
                var value = reader.ReadSingle();
                return Unsafe.As<float, T>(ref value);
            }

            if (typeof(T) == typeof(double))
            {
                var value = reader.ReadDouble();
                return Unsafe.As<double, T>(ref value);
            }

            if (typeof(T) == typeof(Complex))
            {
                var value = new Complex(reader.ReadDouble(), reader.ReadDouble());
                return Unsafe.As<Complex, T>(ref value);
            }

            throw new NotSupportedException("The element type must be float, double, or Complex.");
        }

        private static void WriteValue<T>(BinaryWriter writer, T value) where T : unmanaged, INumberBase<T>
        {
            if (typeof(T) == typeof(float))
            {
                writer.Write(Unsafe.As<T, float>(ref value));
                return;
            }

            if (typeof(T) == typeof(double))
            {
                writer.Write(Unsafe.As<T, double>(ref value));
                return;
            }

            if (typeof(T) == typeof(Complex))
            {
                var complex = Unsafe.As<T, Complex>(ref value);
                writer.Write(complex.Real);
                writer.Write(complex.Imaginary);
                return;
            }

            throw new NotSupportedException("The element type must be float, double, or Complex.");
        }



        private readonly struct NpyHeader
        {
            public NpyHeader(string descriptor, int[] shape)
            {
                Descriptor = descriptor;
                Shape = shape;
            }

            public string Descriptor { get; }

            public int[] Shape { get; }
        }
    }
}
