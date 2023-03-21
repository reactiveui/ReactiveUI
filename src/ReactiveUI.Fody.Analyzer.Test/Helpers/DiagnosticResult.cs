// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace TestHelper
{
    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    public struct DiagnosticResult : IEquatable<DiagnosticResult>
    {
        private IList<DiagnosticResultLocation> _locations;

        /// <summary>
        /// Gets or sets the locations of the Analysis Result.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Deliberate usage.")]
        public IList<DiagnosticResultLocation> Locations
        {
            get => _locations ??= Array.Empty<DiagnosticResultLocation>();

            set => _locations = value;
        }

        /// <summary>
        /// Gets or Sets Severity of the Analysis Result.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the Id of the Analysis Result.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Analysis Result Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the Path of the source file that caused the Analysis Result.
        /// </summary>
        public string Path => Locations.Count > 0 ? Locations[0].Path : string.Empty;

        /// <summary>
        /// Gets the line number of the source that caused the Analysis Result.
        /// </summary>
        public int Line => Locations.Count > 0 ? Locations[0].Line : -1;

        /// <summary>
        /// Gets the column number of the source that caused the Analysis Result.
        /// </summary>
        public int Column => Locations.Count > 0 ? Locations[0].Column : -1;

        /// <summary>
        /// Performs equality against left and right.
        /// </summary>
        /// <param name="left">Left side to compare.</param>
        /// <param name="right">Right side to compare.</param>
        /// <returns>If the two values are equal.</returns>
        public static bool operator ==(DiagnosticResult left, DiagnosticResult right) => left.Equals(right);

        /// <summary>
        /// Performs inequality against left and right.
        /// </summary>
        /// <param name="left">Left side to compare.</param>
        /// <param name="right">Right side to compare.</param>
        /// <returns>If the two values are not equal.</returns>
        public static bool operator !=(DiagnosticResult left, DiagnosticResult right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is DiagnosticResult result && Equals(result);

        /// <inheritdoc/>
        public bool Equals(DiagnosticResult other) =>
            EqualityComparer<IList<DiagnosticResultLocation>>.Default.Equals(_locations, other._locations) &&
            EqualityComparer<IList<DiagnosticResultLocation>>.Default.Equals(Locations, other.Locations) &&
            Severity == other.Severity &&
            Id == other.Id &&
            Message == other.Message &&
            Path == other.Path &&
            Line == other.Line &&
            Column == other.Column;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 1054991603;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IList<DiagnosticResultLocation>>.Default.GetHashCode(_locations);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IList<DiagnosticResultLocation>>.Default.GetHashCode(Locations);
            hashCode = (hashCode * -1521134295) + Severity.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Path);
            hashCode = (hashCode * -1521134295) + Line.GetHashCode();
            hashCode = (hashCode * -1521134295) + Column.GetHashCode();
            return hashCode;
        }
    }
}
