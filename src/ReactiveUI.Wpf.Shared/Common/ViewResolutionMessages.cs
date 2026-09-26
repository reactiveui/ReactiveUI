// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>The messages the Unsafe view-resolution types carry in their ahead-of-time annotations.</summary>
internal static class ViewResolutionMessages
{
    /// <summary>The message for <see cref="ViewModelViewHostUnsafe"/>.</summary>
    internal const string UnsafeHost =
        "ViewModelViewHostUnsafe asks the service locator for IViewFor<> closed over the view model's run-time type. "
        + "Use ViewModelViewHost, which uses the generated view lookup and Map registrations, to stay ahead-of-time safe.";

    /// <summary>The message for <see cref="RoutedViewHostUnsafe"/>.</summary>
    internal const string UnsafeRoutedHost =
        "RoutedViewHostUnsafe asks the service locator for IViewFor<> closed over the view model's run-time type. "
        + "Use RoutedViewHost, which uses the generated view lookup and Map registrations, to stay ahead-of-time safe.";

    /// <summary>The message for <see cref="AutoDataTemplateBindingHookUnsafe"/>.</summary>
    internal const string UnsafeTemplateHook =
        "AutoDataTemplateBindingHookUnsafe hosts each item in a ViewModelViewHostUnsafe, which asks the service locator for "
        + "IViewFor<> closed over the view model's run-time type. Use AutoDataTemplateBindingHook, which uses the generated "
        + "view lookup and Map registrations, to stay ahead-of-time safe.";
}
