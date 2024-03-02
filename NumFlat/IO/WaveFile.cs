using System;
using System.Buffers;
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
                int channels = 0;
                int sampleRate = 0;
                Vec<double>[]? data = null;
                while (true)
                {
                    var id = ReadFourCC(reader);
                    var size = reader.ReadUInt32();
                    switch (id)
                    {
                        case "fmt ":
                            (sampleFormat, channels, sampleRate) = ReadFormat(reader, (int)size);
                            break;
                        case "data":
                            data = ReadData(reader, (int)size, sampleFormat, channels);
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
        /// Reads the specified monaural wave file.
        /// </summary>
        /// <param name="path">
        /// The path of the wave file.
        /// </param>
        /// <returns>
        /// The wave data and sample rate.
        /// </returns>
        public static (Vec<double> Data, int SampleRate) ReadMono(string path)
        {
            var (data, sampleRate) = Read(path);
            return (data[0], sampleRate);
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

        private static (SampleFormat SampleFormat, int Channels, int SampleRate) ReadFormat(BinaryReader reader, int size)
        {
            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var sampleFormat = (SampleFormat)BitConverter.ToInt16(buffer.Slice(0, 2));
            if (!Enum.IsDefined(sampleFormat))
            {
                throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
            }

            var channels = (int)BitConverter.ToInt16(buffer.Slice(2, 2));
            if (channels <= 0)
            {
                throw new InvalidDataException($"Invalid channel count ({channels}).");
            }

            var sampleRate = BitConverter.ToInt32(buffer.Slice(4, 4));
            if (sampleRate <= 0)
            {
                throw new InvalidDataException($"Invalid sample rate ({sampleRate}).");
            }

            var blockAlign = (int)BitConverter.ToInt16(buffer.Slice(12, 2));
            var expected = GetSampleSize(sampleFormat) * channels;
            if (blockAlign != expected)
            {
                throw new InvalidDataException($"Block align is expected to be '{expected}', but was '{blockAlign}'.");
            }

            return (sampleFormat, channels, sampleRate);
        }

        private static Vec<double>[] ReadData(BinaryReader reader, int size, SampleFormat sampleFormat, int channels)
        {
            if (sampleFormat == 0)
            {
                throw new InvalidDataException($"The format chunk was not found.");
            }

            using var ubuffer = MemoryPool<byte>.Shared.Rent(size);
            var buffer = ubuffer.Memory.Span.Slice(0, size);
            reader.Read(buffer);

            var sampleCount = size / (GetSampleSize(sampleFormat) * channels);

            var data = new Vec<double>[channels];
            for (var ch = 0; ch < channels; ch++)
            {
                data[ch] = new Vec<double>(sampleCount);
            }

            for (var ch = 0; ch < channels; ch++)
            {
                if (sampleFormat == SampleFormat.Int16)
                {
                    var src = MemoryMarshal.Cast<byte, short>(buffer);
                    var dst = data[ch].Memory.Span;
                    var position = ch;
                    for (var i = 0; i < dst.Length; i++)
                    {
                        dst[i] = (double)src[position] / 32768;
                        position += channels;
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
                        position += channels;
                    }
                }
                else
                {
                    throw new InvalidDataException($"Unsupported sample format ({(int)sampleFormat}).");
                }
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



        private enum SampleFormat
        {
            Int16 = 1,
            Float32 = 3,
        }
    }
}
