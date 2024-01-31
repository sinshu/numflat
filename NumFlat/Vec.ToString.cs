using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

namespace NumFlat
{
    public partial struct Vec<T>
    {
        private const string omit = "..";
        private const int threshold = 3;
        private const int maxVisibleRowCount = 10;

        /// <summary>
        /// Format the vector as a string suitable for display on a console.
        /// </summary>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Format the vector as a string suitable for display on a console.
        /// </summary>
        /// <param name="format">
        /// <inheritdoc/>
        /// </param>
        /// <param name="provider">
        /// <inheritdoc/>
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public string ToString(string? format, IFormatProvider? provider)
        {
            if (format == null)
            {
                format = "G6";
            }

            if (count == 0)
            {
                return $"Vector Empty-{typeof(T).Name}";
            }

            string[] array;
            int maxLength;
            if (count <= maxVisibleRowCount)
            {
                array = this.Select(x => x.ToString(format, provider)).ToArray();
                maxLength = array.Select(s => s.Length).Max();
            }
            else
            {
                var upper = this.Take(maxVisibleRowCount - threshold).Select(x => x.ToString(format, provider));
                var lower = this.TakeLast(threshold).Select(x => x.ToString(format, provider));
                array = upper.Append(omit).Concat(lower).ToArray();
                maxLength = array.Select(s => s.Length).Max();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Vector {count}-{typeof(T).Name}");
            for (var col = 0; col < array.Length; col++)
            {
                var value = array[col];
                var pad = maxLength - value.Length;
                for (var i = 0; i < pad; i++)
                {
                    sb.Append(' ');
                }
                sb.AppendLine(value);
            }
            return sb.ToString();
        }
    }
}
