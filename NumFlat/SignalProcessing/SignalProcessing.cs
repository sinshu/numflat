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
    }
}
