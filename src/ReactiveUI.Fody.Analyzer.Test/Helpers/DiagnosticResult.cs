// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
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
    public struct DiagnosticResult
    {
        private IList<DiagnosticResultLocation> _locations;

        /// <summary>
        /// Gets or sets the locations of the Analysis Result.
        /// </summary>
        public IList<DiagnosticResultLocation> Locations
        {
            get
            {
                if (_locations == null)
                {
                    _locations = Array.Empty<DiagnosticResultLocation>();
                }

                return _locations;
            }

            set
            {
                _locations = value;
            }
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
        public string Path
        {
            get
            {
                return Locations.Count > 0 ? Locations[0].Path : string.Empty;
            }
        }

        /// <summary>
        /// Gets the line number of the source that caused the Analysis Result.
        /// </summary>
        public int Line
        {
            get
            {
                return Locations.Count > 0 ? Locations[0].Line : -1;
            }
        }

        /// <summary>
        /// Gets the column number of the source that caused the Analysis Result.
        /// </summary>
        public int Column
        {
            get
            {
                return Locations.Count > 0 ? Locations[0].Column : -1;
            }
        }
    }
}
