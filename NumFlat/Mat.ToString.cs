using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        private const string space = "  ";
        private const string omit = "..";
        private const int threshold = 3;
        private const int maxCharCount = 78;
        private const int maxVisibleRowCount = 10;

        /// <summary>
        /// Format the matrix as a string suitable for display on a console.
        /// </summary>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public override string ToString()
        {
            return ToString("G6");
        }

        /// <summary>
        /// Format the matrix as a string suitable for display on a console.
        /// </summary>
        /// <param name="format">
        /// The format to use.
        /// </param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public string ToString(string format)
        {
            if (rowCount == 0 || colCount == 0)
            {
                return $"DenseMatrix Empty-{typeof(T).Name}";
            }

            var header = $"DenseMatrix {rowCount}x{colCount}-{typeof(T).Name}";
            if (colCount <= 2 * threshold)
            {
                var infos = Cols.Select(col => GetStringInfoFromSingleCol(col, format)).ToArray();
                return BuildString(header, infos);
            }
            else
            {
                var left = Cols.Take(threshold).Select(col => GetStringInfoFromSingleCol(col, format)).ToList();
                var right = Cols.TakeLast(threshold).Select(col => GetStringInfoFromSingleCol(col, format)).ToList();

                foreach (var col in Cols.Skip(threshold).SkipLast(threshold))
                {
                    left.Add(GetStringInfoFromSingleCol(col, format));

                    var visibleColCount = left.Count + right.Count;
                    int charCount;
                    if (visibleColCount == colCount)
                    {
                        charCount = left.Select(info => info.width).Sum() + right.Select(info => info.width).Sum()
                            + space.Length * (visibleColCount - 1);
                    }
                    else
                    {
                        charCount = left.Select(info => info.width).Sum() + right.Select(info => info.width).Sum() + omit.Length
                            + space.Length * visibleColCount;
                    }

                    if (charCount > maxCharCount)
                    {
                        left.RemoveAt(left.Count - 1);
                        break;
                    }
                }

                if (left.Count + right.Count < colCount)
                {
                    var lineCount = left[0].values.Length;
                    var width = omit.Length;
                    var values = Enumerable.Range(0, lineCount).Select(i => omit).ToArray();
                    left.Add((width, values));
                }

                return BuildString(header, left.Concat(right).ToArray());
            }
        }

        private static (int width, string[] values) GetStringInfoFromSingleCol(Vec<T> col, string format)
        {
            if (col.Count <= maxVisibleRowCount)
            {
                var array = col.Select(x => x.ToString(format, null)).ToArray();
                var maxLength = array.Select(s => s.Length).Max();
                return (maxLength, array);
            }
            else
            {
                var upper = col.Take(maxVisibleRowCount - threshold).Select(x => x.ToString(format, null));
                var lower = col.TakeLast(threshold).Select(x => x.ToString(format, null));
                var array = upper.Append(omit).Concat(lower).ToArray();
                var maxLength = array.Select(s => s.Length).Max();
                return (maxLength, array);
            }
        }

        private static string BuildString(string header, IReadOnlyList<(int width, string[] values)> infos)
        {
            var sb = new StringBuilder();
            sb.AppendLine(header);

            var rowCount = infos[0].values.Length;
            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < infos.Count; col++)
                {
                    if (col > 0)
                    {
                        sb.Append(space);
                    }

                    var width = infos[col].width;
                    var value = infos[col].values[row];
                    var pad = width - value.Length;
                    for (var i = 0; i < pad; i++)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(value);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
