// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Creates the view-contract streams shared by platform view hosts.</summary>
internal static class ViewContractObservableHelpers
{
    /// <summary>Gets the platform orientation callback used by a view host.</summary>
    /// <param name="logger">The host logger.</param>
    /// <returns>The platform orientation callback.</returns>
    internal static Func<string?> GetPlatformOrientation(IFullLogger logger)
    {
        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        if (platform is null)
        {
            logger.Error(
                "Couldn't find an IPlatformOperations implementation. Please make sure you have installed "
                + "the latest version of the ReactiveUI packages for your platform. "
                + "See https://reactiveui.net/docs/getting-started/installation for guidance.");
            return static () => null;
        }

        return platform.GetOrientation;
    }

    /// <summary>Creates a contract stream from platform orientation and host size changes.</summary>
    /// <param name="platformGetter">Gets the current platform orientation.</param>
    /// <param name="sizeChanges">Signals the current orientation after a host size change.</param>
    /// <returns>The view-contract stream.</returns>
    internal static IObservable<string?> Create(
        Func<string?> platformGetter,
        IObservable<string?> sizeChanges) =>
        ModeDetector.InUnitTestRunner()
            ? Signal.Silent<string?>()
            : new StartWithObservable<string?>(sizeChanges, platformGetter()).DistinctUntilChanged();
}
