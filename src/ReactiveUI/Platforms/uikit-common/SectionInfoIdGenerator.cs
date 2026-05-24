// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Generates unique integer identifiers for section info instances.</summary>
internal static class SectionInfoIdGenerator
{
    /// <summary>The next identifier value to be issued by <see cref="Generate"/>.</summary>
    private static int nextSectionInfoId;

    /// <summary>Returns the next unique section info identifier and increments the internal counter.</summary>
    /// <returns>A unique integer identifier.</returns>
    public static int Generate() => nextSectionInfoId++;
}
