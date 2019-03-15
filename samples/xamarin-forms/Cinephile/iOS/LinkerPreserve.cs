// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Akavache.Sqlite3;

// Note: This class file is *required* for iOS to work correctly, and is
// also a good idea for Android if you enable "Link All Assemblies".
namespace Cinephile.iOS
{
    /// <summary>
    /// An override for the Akavache project.
    /// </summary>
    [Preserve]
    public static class LinkerPreserve
    {
        /// <summary>
        /// Initializes static members of the <see cref="LinkerPreserve"/> class.
        /// </summary>
        [SuppressMessage("Design", "CA1065: .cctor creates an exception of type Exception.", Justification = "Deliberate usage")]
        static LinkerPreserve()
        {
            throw new Exception(typeof(SQLitePersistentBlobCache).FullName);
        }
    }
}
