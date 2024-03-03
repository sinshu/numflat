using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NumFlat.IO
{
    /// <summary>
    /// Provides wave file IO.
    /// </summary>
    public static class WaveFile
    {
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
                if (ReadFourCC(reader) != "RIFF")
                {
                    throw new InvalidDataException("The RIFF chunk was not found.");
                }
                reader.ReadInt32();
                if (ReadFourCC(reader) != "WAVE")
                {
                    throw new InvalidDataException("The file is not a wave file.");
                }

                SampleFormat sampleFormat = 0;
                int channelCount = 0;
                int sampleRate = 0;
                Vec<double>[]? data = null;
                while (true)
                {
                    var id = ReadFourCC(reader);
                    var size = reader.ReadUInt32();
                    if (size % 2 != 0)
                    {
                        size++;
                    }
                    switch (id)
                    {
                        case "fmt ":
                            (sampleFormat, channelCount, sampleRate) = ReadFormat(reader, (int)size);
                            break;
                        case "data":
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
                if (ReadFourCC(reader) != "RIFF")
                {
                    throw new InvalidDataException("The RIFF chunk was not found.");
                }
                reader.ReadInt32();
                if (ReadFourCC(reader) != "WAVE")
                {
                    throw new InvalidDataException("The file is not a wave file.");
                }

                SampleFormat sampleFormat = 0;
                int channelCount = 0;
                int sampleRate = 0;
                Vec<double> data = default;
                while (true)
                {
                    var id = ReadFourCC(reader);
                    var size = reader.ReadUInt32();
                    if (size % 2 != 0)
                    {
                        size++;
                    }
                    switch (id)
                    {
                        case "fmt ":
                            (sampleFormat, channelCount, sampleRate) = ReadFormat(reader, (int)size);
                            break;
                        case "data":
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

        private static string ReadFourCC(BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            for (var i = 0; i < data.Length; i++)
            {
                var value = data[i];
                if (!(32 <= value && value <= 126))
                {
                    data[i] = (byte)'?';
                }
            }
            return Encoding.ASCII.GetString(data, 0, data.Length);
        }

        private static (SampleFormat SampleFormat, int ChannelCount, int SampleRate) ReadFormat(BinaryReader reader, int size)
        {
            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var sampleFormat = (SampleFormat)BitConverter.ToInt16(buffer.Slice(0, 2));
            if (!Enum.IsDefined(sampleFormat))
            {
                throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
            }

            var channelCount = (int)BitConverter.ToInt16(buffer.Slice(2, 2));
            if (channelCount <= 0)
            {
                throw new InvalidDataException($"Invalid channel count ({channelCount}).");
            }

            var sampleRate = BitConverter.ToInt32(buffer.Slice(4, 4));
            if (sampleRate <= 0)
            {
                throw new InvalidDataException($"Invalid sample rate ({sampleRate}).");
            }

            var blockAlign = (int)BitConverter.ToInt16(buffer.Slice(12, 2));
            var expected = GetSampleSize(sampleFormat) * channelCount;
            if (blockAlign != expected)
            {
                throw new InvalidDataException($"Block align is expected to be '{expected}', but was '{blockAlign}'.");
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

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryReader(fs))
            {
                var dataSize = GetSampleSize(SampleFormat.Int16) * data.Count * sampleCount;

            }

            throw new NotImplementedException();
        }



        private enum SampleFormat
        {
            Int16 = 1,
            Float32 = 3,
        }
    }
}
