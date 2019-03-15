// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

// Note: This class file is *required* for iOS to work correctly, and is
// also a good idea for Android if you enable "Link All Assemblies".
namespace Cinephile.Droid
{
    /// <summary>
    /// An override for Akavache to preserve items.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PreserveAttribute : Attribute
    {
    }
}
