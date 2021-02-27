// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace TestHelper
{
    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public readonly struct DiagnosticResultLocation : IEquatable<DiagnosticResultLocation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticResultLocation"/> struct.
        /// </summary>
        /// <param name="path">The Source File where the issues exists.</param>
        /// <param name="line">The line number of the result.</param>
        /// <param name="column">The column of the Result.</param>
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "column must be >= -1");
            }

            Path = path;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Gets Path of the source file, which has issues.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the line number of the issue.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the columns of the issue.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Compares two DiagnosticResultLocation for Equality.
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <returns>Are Equal.</returns>
        public static bool operator ==(DiagnosticResultLocation left, DiagnosticResultLocation right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(DiagnosticResultLocation left, DiagnosticResultLocation right) => !left.Equals(right);

        /// <summary>
        /// Compares two DiagnosticResultLocation for Equality.
        /// </summary>
        /// <param name="other">Other object to compare to.</param>
        /// <returns>Are Equal.</returns>
        public bool Equals(DiagnosticResultLocation other) => string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase) && Line == other.Line && Column == other.Column;

        /// <summary>
        /// Compares two DiagnosticResultLocation for Equality.
        /// </summary>
        /// <param name="obj">Other object to compare to.</param>
        /// <returns>Are Equal.</returns>
        public override bool Equals(object? obj) => obj is DiagnosticResultLocation other && Equals(other);

        /// <summary>
        /// Gets HashCode.
        /// </summary>
        /// <returns>HashCode.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in NET472")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Path is not null ? Path.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ Column;
                return hashCode;
            }
        }
    }
}
