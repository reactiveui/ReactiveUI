﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// Base platform.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.IPlatform" />
    public abstract class BasePlatform : IPlatform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePlatform"/> class.
        /// </summary>
        protected BasePlatform()
        {
            Assemblies = new List<string>();
            CecilSearchDirectories = new List<string>();
        }

        /// <inheritdoc />
        public abstract AutoPlatform Platform { get; }

        /// <inheritdoc />
        public List<string> Assemblies { get; }

        /// <inheritdoc />
        public List<string> CecilSearchDirectories { get; }

        /// <inheritdoc />
        public abstract Task Extract();
    }
}