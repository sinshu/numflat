using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NumFlat.IO
{
    /// <summary>
    /// Provides CSV file IO.
    /// </summary>
    public static class CsvFile
    {
        /// <summary>
        /// Reads a CSV file as a <see cref="Mat{T}"/>.
        /// </summary>
        /// <param name="path">
        /// The path of the CSV file.
        /// </param>
        /// <param name="skipHeader">
        /// The number of lines at the beginning to skip.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Mat{T}"/>.
        /// </returns>
        public static Mat<double> Read(string path, int skipHeader = 0)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));

            var buffer = new List<IEnumerable<double>>();
            foreach (var line in File.ReadLines(path).Skip(skipHeader))
            {
                var split = line.Split(',');
                buffer.Add(split.Select(double.Parse));
            }
            return buffer.RowsToMatrix();
        }

        /// <summary>
        /// Writes a <see cref="Mat{T}"/> as a CSV file.
        /// </summary>
        /// <param name="path">
        /// The path of the CSV file.
        /// </param>
        /// <param name="mat">
        /// The <see cref="Mat{T}"/> to be written.
        /// </param>
        public static void Write(string path, in Mat<double> mat)
        {
            ThrowHelper.ThrowIfNull(path, nameof(path));
            ThrowHelper.ThrowIfEmpty(mat, nameof(mat));

            using (var writer = new StreamWriter(path))
            {
                foreach (var row in mat.Rows)
                {
                    writer.WriteLine(string.Join(',', row));
                }
            }
        }
    }
}
