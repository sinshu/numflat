using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NumFlat.SignalProcessing
{
    /// <summary>
    /// Provides signal processing functionality.
    /// </summary>
    public static class SignalProcessing
    {
        /// <summary>
        /// Gets a frame from a source signal.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="destination">
        /// The destination of the frame.
        /// </param>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static void GetFrame(in this Vec<double> source, int start, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Count)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                source.Subvector(srcStart, copyLength).CopyTo(destination.Subvector(dstStart, copyLength));
            }
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="length">
        /// The length of the frame.
        /// </param>
        /// <returns>
        /// The specified frame.
        /// </returns>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static Vec<double> GetFrame(in this Vec<double> source, int start, int length)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The frame length must be a positive value.");
            }

            var srcStart = start;
            var dstStart = 0;
            var copyLength = length;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            var destination = new Vec<double>(length);

            if (copyLength > 0)
            {
                source.Subvector(srcStart, copyLength).CopyTo(destination.Subvector(dstStart, copyLength));
            }

            return destination;
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <param name="destination">
        /// The destination of the frame.
        /// </param>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static void GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window, in Vec<double> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (window.Count != destination.Count)
            {
                throw new ArgumentException("The length of the window and destination must match.");
            }

            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Count)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd += dst.Stride;
                }
            }
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <returns>
        /// The specified frame.
        /// </returns>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static Vec<double> GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));

            var srcStart = start;
            var dstStart = 0;
            var copyLength = window.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            var destination = new Vec<double>(window.Count);

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd += dst.Stride;
                }
            }

            return destination;
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// The values in the frame are converted to <see cref="Complex"/> with its imaginary part set to zero.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="destination">
        /// The destination of the frame.
        /// </param>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static void GetFrameAsComplex(in this Vec<double> source, int start, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Count)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = ss[ps];
                    ps += src.Stride;
                    pd += dst.Stride;
                }
            }
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// The values in the frame are converted to <see cref="Complex"/> with its imaginary part set to zero.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="length">
        /// The length of the frame.
        /// </param>
        /// <returns>
        /// The specified frame.
        /// </returns>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static Vec<Complex> GetFrameAsComplex(in this Vec<double> source, int start, int length)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The frame length must be a positive value.");
            }

            var srcStart = start;
            var dstStart = 0;
            var copyLength = length;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            var destination = new Vec<Complex>(length);

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = ss[ps];
                    ps += src.Stride;
                    pd += dst.Stride;
                }
            }

            return destination;
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// The values in the frame are converted to <see cref="Complex"/> with its imaginary part set to zero.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <param name="destination">
        /// The destination of the frame.
        /// </param>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static void GetWindowedFrameAsComplex(in this Vec<double> source, int start, in Vec<double> window, in Vec<Complex> destination)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            if (window.Count != destination.Count)
            {
                throw new ArgumentException("The length of the window and destination must match.");
            }

            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Count)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd += dst.Stride;
                }
            }
        }

        /// <summary>
        /// Gets a frame from a source signal.
        /// The values in the frame are converted to <see cref="Complex"/> with its imaginary part set to zero.
        /// </summary>
        /// <param name="source">
        /// The source signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <returns>
        /// The specified frame.
        /// </returns>
        /// <remarks>
        /// Unlike <see cref="Vec{T}.Subvector(int, int)"/>,
        /// it is not necessary for the frame to fit within the range of the source signal.
        /// The source signal is considered to be an infinite sequence of zeros before and after the source signal.
        /// </remarks>
        public static Vec<Complex> GetWindowedFrameAsComplex(in this Vec<double> source, int start, in Vec<double> window)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));

            var srcStart = start;
            var dstStart = 0;
            var copyLength = window.Count;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            var destination = new Vec<Complex>(window.Count);

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Subvector(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst.Memory.Span;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd += dst.Stride;
                }
            }

            return destination;
        }

        /// <summary>
        /// Adds the values of a frame to the specified position of the target signal.
        /// The target signal will be modified.
        /// </summary>
        /// <param name="target">
        /// The target signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame in the target signal.
        /// </param>
        /// <param name="frame">
        /// The frame to add.
        /// </param>
        /// <remarks>
        /// It is not necessary for the frame to fit within the range of the target signal.
        /// A portion of the frames that do not overlap with the target signal will be discarded.
        /// </remarks>
        public static void OverlapAdd(in this Vec<double> target, int start, in Vec<double> frame)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfEmpty(frame, nameof(frame));

            var trgStart = start;
            var frmStart = 0;
            var addLength = frame.Count;

            if (trgStart < 0)
            {
                var trim = -trgStart;
                trgStart += trim;
                frmStart += trim;
                addLength -= trim;
            }

            if (trgStart + addLength > target.Count)
            {
                var trim = trgStart + addLength - target.Count;
                addLength -= trim;
            }

            if (addLength > 0)
            {
                target.Subvector(trgStart, addLength).AddInplace(frame.Subvector(frmStart, addLength));
            }
        }

        /// <summary>
        /// Adds the values of a frame to the specified position of the target signal.
        /// The target signal will be modified.
        /// </summary>
        /// <param name="target">
        /// The target signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame in the target signal.
        /// </param>
        /// <param name="frame">
        /// The frame to add.
        /// Only the real parts are utilized, and the imaginary parts are discarded.
        /// </param>
        /// <remarks>
        /// It is not necessary for the frame to fit within the range of the target signal.
        /// A portion of the frames that do not overlap with the target signal will be discarded.
        /// </remarks>
        public static void OverlapAdd(in this Vec<double> target, int start, in Vec<Complex> frame)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfEmpty(frame, nameof(frame));

            var trgStart = start;
            var frmStart = 0;
            var addLength = frame.Count;

            if (trgStart < 0)
            {
                var trim = -trgStart;
                trgStart += trim;
                frmStart += trim;
                addLength -= trim;
            }

            if (trgStart + addLength > target.Count)
            {
                var trim = trgStart + addLength - target.Count;
                addLength -= trim;
            }

            if (addLength > 0)
            {
                var trg = target.Subvector(trgStart, addLength);
                var frm = frame.Subvector(frmStart, addLength);
                var st = trg.Memory.Span;
                var sf = frm.Memory.Span;
                var pt = 0;
                var pf = 0;
                while (pt < st.Length)
                {
                    st[pt] += sf[pf].Real;
                    pt += trg.Stride;
                    pf += frm.Stride;
                }
            }
        }

        /// <summary>
        /// Adds the values of a frame to the specified position of the target signal.
        /// The target signal will be modified.
        /// </summary>
        /// <param name="target">
        /// The target signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame in the target signal.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <param name="frame">
        /// The frame to add.
        /// </param>
        /// <remarks>
        /// It is not necessary for the frame to fit within the range of the target signal.
        /// A portion of the frames that do not overlap with the target signal will be discarded.
        /// </remarks>
        public static void WindowedOverlapAdd(in this Vec<double> target, int start, in Vec<double> window, in Vec<double> frame)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));
            ThrowHelper.ThrowIfEmpty(frame, nameof(frame));

            if (window.Count != frame.Count)
            {
                throw new ArgumentException("The length of the window and frame must match.");
            }

            var trgStart = start;
            var frmStart = 0;
            var addLength = frame.Count;

            if (trgStart < 0)
            {
                var trim = -trgStart;
                trgStart += trim;
                frmStart += trim;
                addLength -= trim;
            }

            if (trgStart + addLength > target.Count)
            {
                var trim = trgStart + addLength - target.Count;
                addLength -= trim;
            }

            if (addLength > 0)
            {
                var trg = target.Subvector(trgStart, addLength);
                var win = window.Subvector(frmStart, addLength);
                var frm = frame.Subvector(frmStart, addLength);
                var st = trg.Memory.Span;
                var sw = win.Memory.Span;
                var sf = frm.Memory.Span;
                var pt = 0;
                var pw = 0;
                var pf = 0;
                while (pf < sf.Length)
                {
                    st[pt] += sw[pw] * sf[pf];
                    pt += trg.Stride;
                    pw += win.Stride;
                    pf += frm.Stride;
                }
            }
        }

        /// <summary>
        /// Adds the values of a frame to the specified position of the target signal.
        /// The target signal will be modified.
        /// </summary>
        /// <param name="target">
        /// The target signal.
        /// </param>
        /// <param name="start">
        /// The starting position of the frame in the target signal.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to the frame.
        /// </param>
        /// <param name="frame">
        /// The frame to add.
        /// Only the real parts are utilized, and the imaginary parts are discarded.
        /// </param>
        /// <remarks>
        /// It is not necessary for the frame to fit within the range of the target signal.
        /// A portion of the frames that do not overlap with the target signal will be discarded.
        /// </remarks>
        public static void WindowedOverlapAdd(in this Vec<double> target, int start, in Vec<double> window, in Vec<Complex> frame)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));
            ThrowHelper.ThrowIfEmpty(frame, nameof(frame));

            if (window.Count != frame.Count)
            {
                throw new ArgumentException("The length of the window and frame must match.");
            }

            var trgStart = start;
            var frmStart = 0;
            var addLength = frame.Count;

            if (trgStart < 0)
            {
                var trim = -trgStart;
                trgStart += trim;
                frmStart += trim;
                addLength -= trim;
            }

            if (trgStart + addLength > target.Count)
            {
                var trim = trgStart + addLength - target.Count;
                addLength -= trim;
            }

            if (addLength > 0)
            {
                var trg = target.Subvector(trgStart, addLength);
                var win = window.Subvector(frmStart, addLength);
                var frm = frame.Subvector(frmStart, addLength);
                var st = trg.Memory.Span;
                var sw = win.Memory.Span;
                var sf = frm.Memory.Span;
                var pt = 0;
                var pw = 0;
                var pf = 0;
                while (pf < sf.Length)
                {
                    st[pt] += sw[pw] * sf[pf].Real;
                    pt += trg.Stride;
                    pw += win.Stride;
                    pf += frm.Stride;
                }
            }
        }

        /// <summary>
        /// Computes the spectrogram from a time-domain signal using the short-time Fourier transform (STFT).
        /// </summary>
        /// <param name="source">
        /// The source signal to be transformed.
        /// </param>
        /// <param name="window">
        /// The window function to be applied to frames.
        /// </param>
        /// <param name="frameShift">
        /// The frame shift.
        /// </param>
        /// <param name="mode">
        /// The STFT mode.
        /// </param>
        /// <returns>
        /// The spectrogram of the source signal and the information of the transformation.
        /// </returns>
        public static (Vec<Complex>[] Spectrogram, StftInfo Info) Stft(in this Vec<double> source, in Vec<double> window, int frameShift, StftMode mode = StftMode.Analysis)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            ThrowHelper.ThrowIfEmpty(window, nameof(window));

            if (window.Count < 2)
            {
                throw new ArgumentException("The window Length must be greater than or equal to two.", nameof(window));
            }

            if ((window.Count & (window.Count - 1)) != 0)
            {
                throw new ArgumentException($"The window length must be a power of two.", nameof(window));
            }

            if (frameShift <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frameShift), "The frame shift must be a positive value.");
            }

            if (window.Count % frameShift != 0)
            {
                throw new ArgumentException("The window length must be divisible by the frame shift.");
            }

            int firstFramePosition;
            int frameCount;
            if (mode == StftMode.Analysis)
            {
                firstFramePosition = 0;
                frameCount = (source.Count - window.Count) / frameShift;
            }
            else if (mode == StftMode.Synthesis)
            {
                ThrowIfReconstructionIsNotPossible(window, frameShift);
                firstFramePosition = frameShift - window.Count;
                frameCount = (source.Count - firstFramePosition + frameShift - 1) / frameShift;
            }
            else
            {
                throw new ArgumentException("Invalid enum value.", nameof(mode));
            }

            if (frameCount == 0)
            {
                throw new ArgumentException("The length of the source signal is too short.");
            }

            var rft = FourierTransform.GetRftInstance(window.Count);
            var position = firstFramePosition;
            var spectrogram = new Vec<Complex>[frameCount];
            for (var i = 0; i < spectrogram.Length; i++)
            {
                var spectrum = new Vec<Complex>(window.Count / 2 + 1);
                var span = spectrum.Memory.Span;
                source.GetWindowedFrame(position, window, MemoryMarshal.Cast<Complex, double>(span.Slice(0, window.Count / 2)));
                rft.Forward(MemoryMarshal.Cast<Complex, double>(span));
                spectrogram[i] = spectrum;
                position += frameShift;
            }

            var info = new StftInfo(window, firstFramePosition, frameShift, source.Count);

            return (spectrogram, info);
        }

        /// <summary>
        /// Reconstruct the time-domain signal from a spectrogram using the inverse short-time Fourier transform (ISTFT).
        /// </summary>
        /// <param name="spectrogram">
        /// The spectrogram to be inverse transformed.
        /// </param>
        /// <param name="info">
        /// The settings of the STFT used for the original transformation.
        /// </param>
        /// <returns>
        /// The time-domain signal reconstructed from the spectrogram.
        /// </returns>
        public static Vec<double> Istft(this IEnumerable<Vec<Complex>> spectrogram, StftInfo info)
        {
            ThrowHelper.ThrowIfNull(spectrogram, nameof(spectrogram));
            ThrowHelper.ThrowIfNull(info, nameof(info));

            using var uframe = new TemporalVector<double>(info.Window.Count + 2);
            ref readonly var frame = ref uframe.Item;
            var sf = frame.Memory.Span;

            var rft = FourierTransform.GetRftInstance(info.Window.Count);
            var position = info.FirstFramePosition;
            var destination = new Vec<double>(info.SignalLength);
            foreach (var spectrum in spectrogram)
            {
                if (spectrum.Count != info.Window.Count / 2 + 1)
                {
                    throw new ArgumentException("The size of a spectrum is invalid.");
                }

                var csf = MemoryMarshal.Cast<double, Complex>(sf);
                spectrum.CopyTo(csf);
                rft.Inverse(csf);
                destination.WindowedOverlapAdd(position, info.Window, frame.Subvector(0, info.Window.Count));
                position += info.FrameShift;
            }

            destination.MulInplace(1 / GetWindowGain(info.Window, info.FrameShift));

            return destination;
        }

        private static void GetWindowedFrame(in this Vec<double> source, int start, in Vec<double> window, Span<double> destination)
        {
            var srcStart = start;
            var dstStart = 0;
            var copyLength = destination.Length;

            if (srcStart < 0)
            {
                var trim = -srcStart;
                srcStart += trim;
                dstStart += trim;
                copyLength -= trim;
            }

            if (srcStart + copyLength > source.Count)
            {
                var trim = srcStart + copyLength - source.Count;
                copyLength -= trim;
            }

            if (copyLength < destination.Length)
            {
                destination.Clear();
            }

            if (copyLength > 0)
            {
                var src = source.Subvector(srcStart, copyLength);
                var win = window.Subvector(dstStart, copyLength);
                var dst = destination.Slice(dstStart, copyLength);
                var ss = src.Memory.Span;
                var sw = win.Memory.Span;
                var sd = dst;
                var ps = 0;
                var pw = 0;
                var pd = 0;
                while (pd < sd.Length)
                {
                    sd[pd] = sw[pw] * ss[ps];
                    ps += src.Stride;
                    pw += win.Stride;
                    pd++;
                }
            }
        }

        private static void ThrowIfReconstructionIsNotPossible(in Vec<double> window, int frameShift)
        {
            var fw = window.GetUnsafeFastIndexer();
            var min = double.MaxValue;
            var max = double.MinValue;
            for (var i = 0; i < frameShift; i++)
            {
                var height = 0.0;
                for (var j = i; j < window.Count; j += frameShift)
                {
                    var value = fw[j];
                    height += value * value;
                }
                if (height < min)
                {
                    min = height;
                }
                if (height > max)
                {
                    max = height;
                }
            }

            if (max - min > 1.0E-14) // np.finfo(np.float64).resolution * 10
            {
                throw new ArgumentException("Signal reconstruction is not possible with the specified STFT settings.");
            }
        }

        private static double GetWindowGain(in Vec<double> window, int frameShift)
        {
            var fw = window.GetUnsafeFastIndexer();
            var sum = 0.0;
            for (var i = 0; i < frameShift; i++)
            {
                var height = 0.0;
                for (var j = i; j < window.Count; j += frameShift)
                {
                    var value = fw[j];
                    height += value * value;
                }
                sum += height;
            }
            return sum / frameShift;
        }
    }
}
