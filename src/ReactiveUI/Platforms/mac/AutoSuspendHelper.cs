// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;

using Foundation;

namespace ReactiveUI;

/// <summary>
/// <para>
/// AutoSuspend-based App Delegate. To use AutoSuspend with iOS, change your
/// AppDelegate to inherit from this class, then call:
/// </para>
/// <para><c>Locator.Current.GetService.{ISuspensionHost}().SetupDefaultSuspendResume();</c>.</para>
/// <para>This will fetch your SuspensionHost.</para>
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("AutoSuspendHelper uses RxApp properties which require dynamic code generation")]
[RequiresUnreferencedCode("AutoSuspendHelper uses RxApp properties which may require unreferenced code")]
#endif
public class AutoSuspendHelper : IEnableLogger, IDisposable
{
    private readonly Subject<IDisposable> _shouldPersistState = new();
    private readonly Subject<Unit> _isResuming = new();
    private readonly Subject<Unit> _isUnpausing = new();

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
    /// </summary>
    /// <param name="appDelegate">The application delegate.</param>
    public AutoSuspendHelper(NSApplicationDelegate appDelegate)
    {
        Reflection.ThrowIfMethodsNotOverloaded(
                                               nameof(AutoSuspendHelper),
                                               appDelegate,
                                               nameof(ApplicationShouldTerminate),
                                               nameof(DidFinishLaunching),
                                               nameof(DidResignActive),
                                               nameof(DidBecomeActive),
                                               nameof(DidHide));

        RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
        RxApp.SuspensionHost.IsResuming = _isResuming;
        RxApp.SuspensionHost.IsUnpausing = _isUnpausing;
        RxApp.SuspensionHost.ShouldPersistState = _shouldPersistState;

        var untimelyDemise = new Subject<Unit>();
        AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            untimelyDemise.OnNext(Unit.Default);

        RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
    }

    /// <summary>
    /// Applications the should terminate.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <returns>The termination reply from the application.</returns>
    public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
    {
        RxSchedulers.MainThreadScheduler.Schedule(() =>
                                               _shouldPersistState.OnNext(Disposable.Create(() =>
                                                                              sender.ReplyToApplicationShouldTerminate(true))));

        return NSApplicationTerminateReply.Later;
    }

    /// <summary>
    /// Did finish launching.
    /// </summary>
    /// <param name="notification">The notification.</param>
    public void DidFinishLaunching(NSNotification notification) => _isResuming.OnNext(Unit.Default);

    /// <summary>
    /// Did resign active.
    /// </summary>
    /// <param name="notification">The notification.</param>
    public void DidResignActive(NSNotification notification) => _shouldPersistState.OnNext(Disposable.Empty);

    /// <summary>
    /// Did become active.
    /// </summary>
    /// <param name="notification">The notification.</param>
    public void DidBecomeActive(NSNotification notification) => _isUnpausing.OnNext(Unit.Default);

    /// <summary>
    /// Did hide.
    /// </summary>
    /// <param name="notification">The notification.</param>
    public void DidHide(NSNotification notification) => _shouldPersistState.OnNext(Disposable.Empty);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of resources inside the class.
    /// </summary>
    /// <param name="isDisposing">If we are disposing managed resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            _isResuming?.Dispose();
            _isUnpausing?.Dispose();
            _shouldPersistState?.Dispose();
        }

        _isDisposed = true;
    }
}
