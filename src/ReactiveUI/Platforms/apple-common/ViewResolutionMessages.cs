// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>The messages the Apple Unsafe view-resolution hosts carry in their ahead-of-time annotations.</summary>
internal static class ViewResolutionMessages
{
    /// <summary>The message for <c>ViewModelViewHostUnsafe</c>.</summary>
    internal const string UnsafeHost =
        "ViewModelViewHostUnsafe resolves a view by closing IViewFor<> over the view model's run-time type. "
        + "Use ViewModelViewHost, which uses the generated view lookup, to stay ahead-of-time safe.";

    /// <summary>The message for <c>RoutedViewHostUnsafe</c>.</summary>
    internal const string UnsafeRoutedHost =
        "RoutedViewHostUnsafe resolves a view by closing IViewFor<> over the view model's run-time type. "
        + "Use RoutedViewHost, which uses the generated view lookup, to stay ahead-of-time safe.";
}
