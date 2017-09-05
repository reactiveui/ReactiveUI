using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        /// <summary>
        /// Answers true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue(this string s) => !string.IsNullOrEmpty(s);

        /// <summary>
        /// Returns the toReturn parameter when this string is null/empty.
        /// </summary>
        public static string IsNullOrEmptyReturn(this string s, string toReturn) => s.HasValue() ? s : toReturn;

        /// <summary>
        /// force string to be maxlen or smaller
        /// </summary>
        public static string Truncate(this string s, int maxLength) =>
            s.IsNullOrEmpty()
                ? s
                : (s.Length > maxLength ? s.Remove(maxLength) : s);

        /// <summary>
        /// force string to be maxlen or smaller and adds ellipsis
        /// </summary>
        public static string TruncateWithEllipsis(this string s, int maxLength) =>
            s.IsNullOrEmpty() || s.Length <= maxLength
                ? s
                : Truncate(s, Math.Max(maxLength, 3) - 3) + "...";
    }
}
