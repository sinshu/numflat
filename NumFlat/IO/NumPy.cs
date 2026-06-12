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

            var header = ReadHeader(stream);
            if (header.Shape.Length != 1)
            {
                throw new InvalidDataException("The .npy file does not contain a one-dimensional array.");
            }

            var dtype = GetDtype<T>();
            if (header.Descr != dtype)
            {
                throw new InvalidDataException($"The .npy dtype '{header.Descr}' does not match '{dtype}'.");
            }

            var vector = new Vec<T>(header.Shape[0]);
            var fv = vector.GetUnsafeFastIndexer();
            for (var i = 0; i < vector.Count; i++)
            {
                fv[i] = ReadValue<T>(stream);
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

            var header = ReadHeader(stream);
            if (header.Shape.Length != 2)
            {
                throw new InvalidDataException("The .npy file does not contain a two-dimensional array.");
            }

            var dtype = GetDtype<T>();
            if (header.Descr != dtype)
            {
                throw new InvalidDataException($"The .npy dtype '{header.Descr}' does not match '{dtype}'.");
            }

            var matrix = new Mat<T>(header.Shape[0], header.Shape[1]);
            var fm = matrix.GetUnsafeFastIndexer();
            for (var row = 0; row < matrix.RowCount; row++)
            {
                for (var col = 0; col < matrix.ColCount; col++)
                {
                    fm[row, col] = ReadValue<T>(stream);
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

            WriteHeader(stream, GetDtype<T>(), vector.Count);
            var fv = vector.GetUnsafeFastIndexer();
            for (var i = 0; i < vector.Count; i++)
            {
                WriteValue(stream, fv[i]);
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

            WriteHeader(stream, GetDtype<T>(), matrix.RowCount, matrix.ColCount);
            var fm = matrix.GetUnsafeFastIndexer();
            for (var row = 0; row < matrix.RowCount; row++)
            {
                for (var col = 0; col < matrix.ColCount; col++)
                {
                    WriteValue(stream, fm[row, col]);
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

        private static NpyHeader ReadHeader(Stream stream)
        {
            var magic = ReadBytes(stream, Magic.Length);
            for (var i = 0; i < Magic.Length; i++)
            {
                if (magic[i] != Magic[i])
                {
                    throw new InvalidDataException("The stream does not contain a NumPy .npy file.");
                }
            }

            var major = stream.ReadByte();
            var minor = stream.ReadByte();
            if (major < 0 || minor < 0)
            {
                throw new EndOfStreamException();
            }

            var headerLength = major switch
            {
                1 => ReadUInt16(stream),
                2 => ReadInt32(stream),
                _ => throw new NotSupportedException($"NumPy .npy version {major}.{minor} is not supported."),
            };

            var header = Encoding.ASCII.GetString(ReadBytes(stream, headerLength));
            return ParseHeader(header);
        }

        private static NpyHeader ParseHeader(string header)
        {
            var descrMatch = Regex.Match(header, "'descr'\\s*:\\s*'(?<descr>[^']+)'");
            if (!descrMatch.Success)
            {
                throw new InvalidDataException("The .npy header does not contain 'descr'.");
            }

            if (!Regex.IsMatch(header, "'fortran_order'\\s*:\\s*False"))
            {
                throw new NotSupportedException("Only C-order .npy arrays are supported.");
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
                    throw new NotSupportedException("Empty .npy arrays are not supported.");
                }
            }

            return new NpyHeader(descrMatch.Groups["descr"].Value, shape);
        }

        private static void WriteHeader(Stream stream, string dtype, params int[] shape)
        {
            stream.Write(Magic, 0, Magic.Length);
            stream.WriteByte(1);
            stream.WriteByte(0);

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

            WriteUInt16(stream, (ushort)headerBytes.Length);
            stream.Write(headerBytes, 0, headerBytes.Length);
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

            throw new NotSupportedException("Only float, double, and Complex are supported.");
        }

        private static T ReadValue<T>(Stream stream) where T : unmanaged, INumberBase<T>
        {
            if (typeof(T) == typeof(float))
            {
                var value = BitConverter.ToSingle(ReadBytes(stream, sizeof(float)), 0);
                return Unsafe.As<float, T>(ref value);
            }

            if (typeof(T) == typeof(double))
            {
                var value = BitConverter.ToDouble(ReadBytes(stream, sizeof(double)), 0);
                return Unsafe.As<double, T>(ref value);
            }

            if (typeof(T) == typeof(Complex))
            {
                var real = BitConverter.ToDouble(ReadBytes(stream, sizeof(double)), 0);
                var imaginary = BitConverter.ToDouble(ReadBytes(stream, sizeof(double)), 0);
                var value = new Complex(real, imaginary);
                return Unsafe.As<Complex, T>(ref value);
            }

            throw new NotSupportedException("Only float, double, and Complex are supported.");
        }

        private static void WriteValue<T>(Stream stream, T value) where T : unmanaged, INumberBase<T>
        {
            if (typeof(T) == typeof(float))
            {
                var typed = Unsafe.As<T, float>(ref value);
                var bytes = BitConverter.GetBytes(typed);
                stream.Write(bytes, 0, bytes.Length);
                return;
            }

            if (typeof(T) == typeof(double))
            {
                var typed = Unsafe.As<T, double>(ref value);
                var bytes = BitConverter.GetBytes(typed);
                stream.Write(bytes, 0, bytes.Length);
                return;
            }

            if (typeof(T) == typeof(Complex))
            {
                var complex = Unsafe.As<T, Complex>(ref value);
                var real = BitConverter.GetBytes(complex.Real);
                var imaginary = BitConverter.GetBytes(complex.Imaginary);
                stream.Write(real, 0, real.Length);
                stream.Write(imaginary, 0, imaginary.Length);
                return;
            }

            throw new NotSupportedException("Only float, double, and Complex are supported.");
        }

        private static byte[] ReadBytes(Stream stream, int count)
        {
            var bytes = new byte[count];
            var offset = 0;
            while (offset < bytes.Length)
            {
                var read = stream.Read(bytes, offset, bytes.Length - offset);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }
                offset += read;
            }
            return bytes;
        }

        private static ushort ReadUInt16(Stream stream)
        {
            var bytes = ReadBytes(stream, sizeof(ushort));
            return (ushort)(bytes[0] | (bytes[1] << 8));
        }

        private static int ReadInt32(Stream stream)
        {
            var bytes = ReadBytes(stream, sizeof(int));
            return bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24);
        }

        private static void WriteUInt16(Stream stream, ushort value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
        }

        private readonly struct NpyHeader
        {
            public NpyHeader(string descr, int[] shape)
            {
                Descr = descr;
                Shape = shape;
            }

            public string Descr { get; }

            public int[] Shape { get; }
        }
    }
}
