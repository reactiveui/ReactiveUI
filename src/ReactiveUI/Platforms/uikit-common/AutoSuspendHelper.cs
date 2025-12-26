// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

using UIKit;

using NSAction = System.Action;

namespace ReactiveUI;

/// <summary>
/// Bridges iOS lifecycle notifications into <see cref="RxApp.SuspensionHost"/> so applications can persist and
/// restore state without manually wiring UIKit events.
/// </summary>
/// <remarks>
/// <para>
/// Instantiate <see cref="AutoSuspendHelper"/> inside your <see cref="UIApplicationDelegate"/> and forward the
/// <c>FinishedLaunching</c>, <c>OnActivated</c>, and <c>DidEnterBackground</c> events to the helper. The helper updates
/// the shared <see cref="ISuspensionHost"/> observables and takes care of requesting background time when persisting
/// application state.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class AppDelegate : UIApplicationDelegate
/// {
///     private AutoSuspendHelper? _autoSuspendHelper;
///
///     public override bool FinishedLaunching(UIApplication app, NSDictionary options)
///     {
///         _autoSuspendHelper = new AutoSuspendHelper(this);
///         _autoSuspendHelper.FinishedLaunching(app, options);
///         RxApp.SuspensionHost.SetupDefaultSuspendResume();
///         return true;
///     }
///
///     public override void OnActivated(UIApplication application) =>
///         _autoSuspendHelper?.OnActivated(application);
///
///     public override void DidEnterBackground(UIApplication application) =>
///         _autoSuspendHelper?.DidEnterBackground(application);
/// }
/// ]]>
/// </code>
/// </example>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("AutoSuspendHelper uses RxApp.SuspensionHost and reflection which require dynamic code generation")]
[RequiresUnreferencedCode("AutoSuspendHelper uses RxApp.SuspensionHost and reflection which may require unreferenced code")]
#endif
public class AutoSuspendHelper : IEnableLogger, IDisposable
{
    private readonly Subject<UIApplication> _finishedLaunching = new();
    private readonly Subject<UIApplication> _activated = new();
    private readonly Subject<UIApplication> _backgrounded = new();

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
    /// </summary>
    /// <param name="appDelegate">The uiappdelegate.</param>
    public AutoSuspendHelper(UIApplicationDelegate appDelegate)
    {
        Reflection.ThrowIfMethodsNotOverloaded(
                                               nameof(AutoSuspendHelper),
                                               typeof(UIApplicationDelegate),
                                               nameof(FinishedLaunching),
                                               nameof(OnActivated),
                                               nameof(DidEnterBackground));

        RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
        RxApp.SuspensionHost.IsResuming = _finishedLaunching.Select(_ => Unit.Default);
        RxApp.SuspensionHost.IsUnpausing = _activated.Select(_ => Unit.Default);

        var untimelyDeath = new Subject<Unit>();
        AppDomain.CurrentDomain.UnhandledException += (o, e) => untimelyDeath.OnNext(Unit.Default);

        RxApp.SuspensionHost.ShouldInvalidateState = untimelyDeath;

        RxApp.SuspensionHost.ShouldPersistState = _backgrounded.SelectMany(app =>
        {
            var taskId = app.BeginBackgroundTask(new NSAction(() => untimelyDeath.OnNext(Unit.Default)));

            // NB: We're being force-killed, signal invalidate instead
            if (taskId == UIApplication.BackgroundTaskInvalid)
            {
                untimelyDeath.OnNext(Unit.Default);
                return Observable<IDisposable>.Empty;
            }

            return Observable.Return(Disposable.Create(() => app.EndBackgroundTask(taskId)));
        });
    }

    /// <summary>
    /// Gets the launch options captured from the most recent call to <see cref="FinishedLaunching"/>. Keys are converted
    /// to strings and values are stringified for convenience when hydrating state.
    /// </summary>
    public IDictionary<string, string>? LaunchOptions { get; private set; }

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.FinishedLaunching(UIApplication, NSDictionary)"/> was
    /// invoked so it can propagate the <see cref="ISuspensionHost.IsResuming"/> observable.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="launchOptions">The launch options.</param>
    public void FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        LaunchOptions = launchOptions is not null
                            ? launchOptions.Keys.ToDictionary(k => k.ToString(), v => launchOptions[v].ToString())
                            : [];

        // NB: This is run in-context (i.e. not scheduled), so by the time this
        // statement returns, UIWindow should be created already
        _finishedLaunching.OnNext(application);
    }

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.OnActivated(UIApplication)"/> occurred.
    /// </summary>
    /// <param name="application">The application.</param>
    public void OnActivated(UIApplication application) => _activated.OnNext(application);

    /// <summary>
    /// Notifies the helper that <see cref="UIApplicationDelegate.DidEnterBackground(UIApplication)"/> was raised so that
    /// persistence can begin.
    /// </summary>
    /// <param name="application">The application.</param>
    public void DidEnterBackground(UIApplication application) => _backgrounded.OnNext(application);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed resources held by the helper.
    /// </summary>
    /// <param name="isDisposing">If we are going to call Dispose methods on field items.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            _activated?.Dispose();
            _backgrounded?.Dispose();
            _finishedLaunching?.Dispose();
        }

        _isDisposed = true;
    }
}
