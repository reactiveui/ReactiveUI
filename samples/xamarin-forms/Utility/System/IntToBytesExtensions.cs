using System;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// http://stackoverflow.com/questions/128618/c-file-size-format-provider/3968504#3968504
    /// </summary>
    public static class IntToBytesExtensions
    {
        private const int DefaultPrecision = 2;
        private static readonly IList<string> Units = new List<string> { "", "K", "M", "G", "T" };

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToFileSize(this int bytes, string unit = "B", int precision = DefaultPrecision, string zero = "n/a") =>
            ToFileSize((double)bytes, unit, precision, zero: zero);

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToFileSize(this long bytes, string unit = "B", int precision = DefaultPrecision, string zero = "n/a") =>
            ToFileSize((double)bytes, unit, precision, zero: zero);

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToFileSize(this float bytes, string unit = "B", int precision = DefaultPrecision, string zero = "n/a") =>
            ToFileSize((double)bytes, unit, precision, zero: zero);

        /// <summary>
        /// Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <param name="unit">Unit to use in the fomat, defaults to B for bytes</param>
        /// <param name="precision">How much precision to show, defaults to 2</param>
        /// <param name="kiloSize">1k size, usually 1024 or 1000 depending on context</param>
        /// <param name="zero">String to show if the value is 0</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToFileSize(this double bytes, string unit = "B", int precision = DefaultPrecision, int kiloSize = 1024, string zero = "n/a")
        {
            if (bytes < 1) return zero;
            var pow = Math.Floor((bytes > 0 ? Math.Log(bytes) : 0) / Math.Log(kiloSize));
            pow = Math.Min(pow, Units.Count - 1);
            var value = bytes / Math.Pow(kiloSize, pow);
            return value.ToString(pow == 0 ? "F0" : "F" + precision.ToString()) + " " + Units[(int)pow] + unit;
        }
    }
}
