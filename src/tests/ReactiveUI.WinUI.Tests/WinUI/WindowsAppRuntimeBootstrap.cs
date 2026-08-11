// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Windows.ApplicationModel.DynamicDependency;
using TUnit.Core.Exceptions;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Resolves the Windows App Runtime framework package for this unpackaged test host.</summary>
/// <remarks>
/// <para>
/// Nothing in the Windows App SDK — XAML, the dispatcher queue, any WinRT activation — works until a matching
/// framework package has been located and added to the process's package graph. An MSIX app gets that from its
/// manifest; an unpackaged host like this one has to ask the bootstrapper for it.
/// </para>
/// <para>
/// The SDK will do that from a module initializer, but a module initializer runs before <c>Main</c> and, on a
/// machine with no runtime installed, calls <see cref="Environment.Exit(int)"/> with the failure HRESULT. The
/// test host then dies before the platform has discovered a single test, which is reported as a zero-test run
/// and a non-zero exit code rather than as a missing prerequisite. The project therefore turns the automatic
/// initializer off (<c>WindowsAppSdkBootstrapInitialize=false</c>) and initializes here instead, where a
/// failure is a value rather than a process exit — and where the tests that need the runtime can be skipped
/// with the bootstrapper's own diagnosis attached.
/// </para>
/// </remarks>
internal static class WindowsAppRuntimeBootstrap
{
    /// <summary>The reason the runtime is unusable, or <see langword="null"/> once it has been resolved.</summary>
    /// <remarks>Initialization happens once, on first use, from whichever test reaches the XAML host first.</remarks>
    private static readonly Lazy<string?> Failure = new(Initialize);

    /// <summary>Skips the running test unless the Windows App Runtime is available to it.</summary>
    /// <exception cref="SkipTestException">No usable Windows App Runtime was found.</exception>
    internal static void SkipIfUnavailable()
    {
        if (Failure.Value is not { } reason)
        {
            return;
        }

        Skip.Test(reason);
    }

    /// <summary>Asks the bootstrapper for a framework package matching the SDK this assembly was built against.</summary>
    /// <returns><see langword="null"/> on success, otherwise a description of the failure.</returns>
    private static string? Initialize()
    {
        // InitializeOptions.None matters: the bootstrapper's default is OnNoMatch_ShowUI, which puts a dialog on
        // screen and waits, wedging an unattended run forever instead of reporting that nothing matched.
        try
        {
            var minimumRuntimeVersion = new PackageVersion(Microsoft.WindowsAppSDK.Runtime.Version.UInt64);
            var initialized = Bootstrap.TryInitialize(
                Microsoft.WindowsAppSDK.Release.MajorMinor,
                Microsoft.WindowsAppSDK.Release.VersionTag,
                minimumRuntimeVersion,
                Bootstrap.InitializeOptions.None,
                out var hresult);

            return initialized ? null : DescribeUnresolvedPackage(hresult);
        }
        catch (DllNotFoundException ex)
        {
            return DescribeLoadFailure(ex);
        }
        catch (EntryPointNotFoundException ex)
        {
            return DescribeLoadFailure(ex);
        }
        catch (BadImageFormatException ex)
        {
            return DescribeLoadFailure(ex);
        }
    }

    /// <summary>Describes a bootstrapper that ran but matched no installed framework package.</summary>
    /// <param name="hresult">The failure the bootstrapper reported.</param>
    /// <returns>The description reported as the skip reason.</returns>
    private static string DescribeUnresolvedPackage(int hresult) =>
        $"No Windows App Runtime framework package matching version {Microsoft.WindowsAppSDK.Runtime.Version.DotQuadString} "
        + $"could be resolved (HRESULT 0x{hresult:X8}). Install the Windows App SDK runtime to run the WinUI tests.";

    /// <summary>Describes a failure to load the native bootstrapper itself.</summary>
    /// <param name="error">The load failure.</param>
    /// <returns>The description reported as the skip reason.</returns>
    private static string DescribeLoadFailure(Exception error) =>
        $"The Windows App Runtime bootstrapper could not be loaded: {error.Message}";
}
