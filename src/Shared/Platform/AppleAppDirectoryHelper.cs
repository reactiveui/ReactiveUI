// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif

/// <summary>Creates application storage directories under Apple platform search paths.</summary>
internal static class AppleAppDirectoryHelper
{
    /// <summary>Creates (if necessary) and returns the application storage directory beneath the specified special folder.</summary>
    /// <param name="targetDir">The platform search path directory.</param>
    /// <param name="subDir">The application-specific subdirectory name.</param>
    /// <returns>The fully qualified directory path.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the platform path cannot be resolved or the bundle identifier is unavailable.</exception>
    internal static string CreateAppDirectory(NSSearchPathDirectory targetDir, string subDir)
    {
        var fm = new NSFileManager();

        var url = fm.GetUrl(targetDir, NSSearchPathDomain.All, null, true, out _)
            ?? throw new InvalidOperationException("Unable to resolve platform application support directory.");

        var bundleId = NSBundle.MainBundle?.BundleIdentifier;
        if (string.IsNullOrEmpty(bundleId))
        {
            throw new InvalidOperationException("Unable to resolve application bundle identifier.");
        }

        var basePath = url.RelativePath;
        if (string.IsNullOrEmpty(basePath))
        {
            throw new InvalidOperationException("Resolved application support path was empty.");
        }

        var path = Path.Combine(basePath, bundleId, subDir);

        if (!Directory.Exists(path))
        {
            _ = Directory.CreateDirectory(path);
        }

        return path;
    }
}
