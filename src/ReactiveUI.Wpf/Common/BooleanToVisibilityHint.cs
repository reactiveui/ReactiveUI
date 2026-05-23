// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI;
#endif

/// <summary>
/// Enum that hints at the visibility of a ui element.
/// </summary>
[Flags]
[SuppressMessage(
    "Minor Code Smell",
    "S2342:Enumeration types should comply with a naming convention",
    Justification = "Established public API; renaming is breaking.")]
public enum BooleanToVisibilityHint
{
    /// <summary>
    /// Do not modify the boolean type conversion from it's default action of using the Visibility.Collapsed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Inverse the action of the boolean type conversion, when it's true collapse the visibility.
    /// </summary>
    Inverse = 1 << 1,

#if !HAS_UNO
    /// <summary>
    /// Use the hidden version rather than the Collapsed.
    /// </summary>
    UseHidden = 1 << 2,
#endif
}
