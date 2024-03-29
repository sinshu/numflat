using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NumFlat.IO
{
    /// <summary>
    /// Provides wave file IO.
    /// </summary>
    public static class WaveFile
    {
        private const int riffHeader = 0x46464952;
        private const int waveHeader = 0x45564157;
        private const int fmtHeader = 0x20746D66;
        private const int dataHeader = 0x61746164;

        /// <summary>
        /// Reads the specified wave file.
        /// </summary>
        /// <param name="path">
        /// The path of the wave file.
        /// </param>
        /// <returns>
        /// The wave data and sample rate.
        /// </returns>
        public static (Vec<double>[] Data, int SampleRate) Read(string path)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                if (reader.ReadInt32() != riffHeader)
                {
                    throw new InvalidDataException("The RIFF chunk was not found.");
                }
                reader.ReadInt32();
                if (reader.ReadInt32() != waveHeader)
                {
                    throw new InvalidDataException("The file is not a wave file.");
                }

                SampleFormat sampleFormat = 0;
                int channelCount = 0;
                int sampleRate = 0;
                Vec<double>[]? data = null;
                while (true)
                {
                    var id = reader.ReadInt32();
                    var size = reader.ReadUInt32();
                    if (size % 2 != 0)
                    {
                        size++;
                    }
                    switch (id)
                    {
                        case fmtHeader:
                            (sampleFormat, channelCount, sampleRate) = ReadFormat(reader, (int)size);
                            break;
                        case dataHeader:
                            data = ReadData(reader, (int)size, sampleFormat, channelCount);
                            goto End;
                        default:
                            reader.BaseStream.Position += size;
                            break;
                    }
                }

            End:
                if (data == null)
                {
                    throw new InvalidDataException("The data chunk was not found.");
                }
                return (data, sampleRate);
            }
        }

        /// <summary>
        /// Reads a monaural waveform from the specified wave file.
        /// </summary>
        /// <param name="path">
        /// The path of the wave file.
        /// </param>
        /// <param name="channel">
        /// The channel to be read.
        /// </param>
        /// <returns>
        /// The wave data and sample rate.
        /// </returns>
        public static (Vec<double> Data, int SampleRate) ReadMono(string path, int channel = 0)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                if (reader.ReadInt32() != riffHeader)
                {
                    throw new InvalidDataException("The RIFF chunk was not found.");
                }
                reader.ReadInt32();
                if (reader.ReadInt32() != waveHeader)
                {
                    throw new InvalidDataException("The file is not a wave file.");
                }

                SampleFormat sampleFormat = 0;
                int channelCount = 0;
                int sampleRate = 0;
                Vec<double> data = default;
                while (true)
                {
                    var id = reader.ReadInt32();
                    var size = reader.ReadUInt32();
                    if (size % 2 != 0)
                    {
                        size++;
                    }
                    switch (id)
                    {
                        case fmtHeader:
                            (sampleFormat, channelCount, sampleRate) = ReadFormat(reader, (int)size);
                            break;
                        case dataHeader:
                            data = ReadDataSingleChannel(reader, (int)size, sampleFormat, channelCount, channel);
                            goto End;
                        default:
                            reader.BaseStream.Position += size;
                            break;
                    }
                }

            End:
                if (data.Count == 0)
                {
                    throw new InvalidDataException("The data chunk was not found.");
                }
                return (data, sampleRate);
            }
        }

        private static (SampleFormat SampleFormat, int ChannelCount, int SampleRate) ReadFormat(BinaryReader reader, int size)
        {
            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var formatTag = BitConverter.ToInt16(buffer.Slice(0, 2));
            var bitsPerSample = BitConverter.ToInt16(buffer.Slice(14, 2));
            SampleFormat sampleFormat;
            if (formatTag == 1 && bitsPerSample == 16)
            {
                sampleFormat = SampleFormat.Int16;
            }
            else if (formatTag == 3 && bitsPerSample == 32)
            {
                sampleFormat = SampleFormat.Float32;
            }
            else
            {
                throw new InvalidDataException("Unsupported sample format.");
            }

            var channelCount = (int)BitConverter.ToInt16(buffer.Slice(2, 2));
            if (channelCount <= 0)
            {
                throw new InvalidDataException("Invalid channel count.");
            }

            var sampleRate = BitConverter.ToInt32(buffer.Slice(4, 4));
            if (sampleRate <= 0)
            {
                throw new InvalidDataException("Invalid sample rate.");
            }

            var blockAlign = (int)BitConverter.ToInt16(buffer.Slice(12, 2));
            if (blockAlign != GetSampleSize(sampleFormat) * channelCount)
            {
                throw new InvalidDataException("Invalid block align.");
            }

            return (sampleFormat, channelCount, sampleRate);
        }

        private static Vec<double>[] ReadData(BinaryReader reader, int size, SampleFormat sampleFormat, int channelCount)
        {
            if (sampleFormat == 0)
            {
                throw new InvalidDataException($"The format chunk was not found.");
            }

            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var sampleCount = size / (GetSampleSize(sampleFormat) * channelCount);
            var data = new Vec<double>[channelCount];
            for (var ch = 0; ch < channelCount; ch++)
            {
                data[ch] = new Vec<double>(sampleCount);
            }

            for (var ch = 0; ch < channelCount; ch++)
            {
                if (sampleFormat == SampleFormat.Int16)
                {
                    var src = MemoryMarshal.Cast<byte, short>(buffer);
                    var dst = data[ch].Memory.Span;
                    var position = ch;
                    for (var i = 0; i < dst.Length; i++)
                    {
                        dst[i] = (double)src[position] / 32768;
                        position += channelCount;
                    }
                }
                else if (sampleFormat == SampleFormat.Float32)
                {
                    var src = MemoryMarshal.Cast<byte, float>(buffer);
                    var dst = data[ch].Memory.Span;
                    var position = ch;
                    for (var i = 0; i < dst.Length; i++)
                    {
                        dst[i] = src[position];
                        position += channelCount;
                    }
                }
                else
                {
                    throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
                }
            }

            return data;
        }

        private static Vec<double> ReadDataSingleChannel(BinaryReader reader, int size, SampleFormat sampleFormat, int channelCount, int channel)
        {
            if (sampleFormat == 0)
            {
                throw new InvalidDataException($"The format chunk was not found.");
            }

            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var sampleCount = size / (GetSampleSize(sampleFormat) * channelCount);
            var data = new Vec<double>(sampleCount);

            if (sampleFormat == SampleFormat.Int16)
            {
                var src = MemoryMarshal.Cast<byte, short>(buffer);
                var dst = data.Memory.Span;
                var position = channel;
                for (var i = 0; i < dst.Length; i++)
                {
                    dst[i] = (double)src[position] / 32768;
                    position += channelCount;
                }
            }
            else if (sampleFormat == SampleFormat.Float32)
            {
                var src = MemoryMarshal.Cast<byte, float>(buffer);
                var dst = data.Memory.Span;
                var position = channel;
                for (var i = 0; i < dst.Length; i++)
                {
                    dst[i] = src[position];
                    position += channelCount;
                }
            }
            else
            {
                throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
            }

            return data;
        }

        private static int GetSampleSize(SampleFormat sampleFormat)
        {
            switch (sampleFormat)
            {
                case SampleFormat.Int16:
                    return 2;
                case SampleFormat.Float32:
                    return 4;
                default:
                    throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
            }
        }

        /// <summary>
        /// Writes the data as a wave file.
        /// </summary>
        /// <param name="path">
        /// The path of the wave file.
        /// </param>
        /// <param name="data">
        /// The data to be written.
        /// </param>
        /// <param name="sampleRate">
        /// The sample rate.
        /// </param>
        public static void Write(string path, IReadOnlyList<Vec<double>> data, int sampleRate)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));
            ThrowHelper.ThrowIfNull(data, nameof(data));

            if (data.Count == 0)
            {
                throw new ArgumentException("The data must contain at least one channel.");
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException("The sample rate must be a positive value.");
            }

            var sampleCount = 0;
            foreach (var channel in data.ThrowIfEmptyOrDifferentSize(nameof(data)))
            {
                sampleCount = channel.Count;
            }

            var dataSize = GetSampleSize(SampleFormat.Int16) * data.Count * sampleCount;

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                using var ubuffer = MemoryPool<byte>.Shared.Rent(dataSize);
                var buffer = ubuffer.Memory.Span.Slice(0, dataSize);

                var dst = MemoryMarshal.Cast<byte, short>(buffer);
                for (var ch = 0; ch < data.Count; ch++)
                {
                    var position = ch;
                    foreach (var value in data[ch].GetUnsafeFastIndexer())
                    {
                        var sample = Math.Clamp((int)(value * 32768), short.MinValue, short.MaxValue);
                        dst[position] = (short)sample;
                        position += data.Count;
                    }
                }

                writer.Write(riffHeader);
                writer.Write(dataSize + 36);
                writer.Write(waveHeader);
                writer.Write(fmtHeader);
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)data.Count);
                writer.Write(sampleRate);
                writer.Write(2 * data.Count * sampleRate);
                writer.Write((short)(2 * data.Count));
                writer.Write((short)16);
                writer.Write(dataHeader);
                writer.Write(dataSize);
                writer.Write(buffer);
            }
        }

        /// <summary>
        /// Writes the data as a wave file.
        /// </summary>
        /// <param name="path">
        /// The path of the wave file.
        /// </param>
        /// <param name="data">
        /// The data to be written.
        /// </param>
        /// <param name="sampleRate">
        /// The sample rate.
        /// </param>
        public static void Write(string path, Vec<double> data, int sampleRate)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));
            ThrowHelper.ThrowIfEmpty(data, nameof(data));

            if (sampleRate <= 0)
            {
                throw new ArgumentException("The sample rate must be a positive value.");
            }

            var dataSize = GetSampleSize(SampleFormat.Int16) * data.Count;

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                using var ubuffer = MemoryPool<byte>.Shared.Rent(dataSize);
                var buffer = ubuffer.Memory.Span.Slice(0, dataSize);

                var dst = MemoryMarshal.Cast<byte, short>(buffer);
                var position = 0;
                foreach (var value in data.GetUnsafeFastIndexer())
                {
                    var sample = Math.Clamp((int)(value * 32768), short.MinValue, short.MaxValue);
                    dst[position] = (short)sample;
                    position++;
                }

                writer.Write(riffHeader);
                writer.Write(dataSize + 36);
                writer.Write(waveHeader);
                writer.Write(fmtHeader);
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(sampleRate);
                writer.Write(2 * sampleRate);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(dataHeader);
                writer.Write(dataSize);
                writer.Write(buffer);
            }
        }



        private enum SampleFormat
        {
            Int16 = 1,
            Float32 = 3,
        }
    }
}
