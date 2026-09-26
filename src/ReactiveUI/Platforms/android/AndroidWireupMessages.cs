// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>The messages the Android Unsafe view hosts carry in their trimming and ahead-of-time annotations.</summary>
internal static class AndroidWireupMessages
{
    /// <summary>The message for <c>LayoutViewHostUnsafe</c> and <c>ReactiveViewHostUnsafe</c>.</summary>
    internal const string UnsafeWireup =
        "Auto-wireup finds the host's properties and the app's Android resource ids by reflection, which the trimmer cannot see. "
        + "Use LayoutViewHost or ReactiveViewHost and wire controls in the bind callback to stay trim and ahead-of-time safe.";
}
