// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using Application = Microsoft.Maui.Controls.Application;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>Helps manage .NET MAUI application lifecycle events.</summary>
/// <remarks>
/// <para>
/// Instantiate this class to wire <see cref="RxSuspension.SuspensionHost"/> to MAUI's <see cref="Application"/>
/// callbacks. The helper propagates <c>OnStart</c>, <c>OnResume</c>, and <c>OnSleep</c> to the suspension host so state
/// drivers created via <c>SetupDefaultSuspendResume</c> can serialize view models consistently across Android, iOS, and
/// desktop targets.
/// </para>
/// <para>
/// Sample usage of <see cref="AutoSuspendHelper"/> is shown below.
/// <code>
/// <![CDATA[
/// public partial class App : Application
/// {
///   private readonly AutoSuspendHelper _autoSuspendHelper;
///   public App()
///   {
///     _autoSuspendHelper = new AutoSuspendHelper();
///     RxSuspension.SuspensionHost.CreateNewAppState = () => new MainState();
///     RxSuspension.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(FileSystem.AppDataDirectory));
///     _autoSuspendHelper.OnCreate();
///
///     InitializeComponent();
///     MainPage = new MainView();
///   }
///   protected override void OnStart() => _autoSuspendHelper.OnStart();
///   protected override void OnResume() => _autoSuspendHelper.OnResume();
///   protected override void OnSleep() => _autoSuspendHelper.OnSleep();
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
public class AutoSuspendHelper : IEnableLogger, IDisposable
{
    /// <summary>Signals that the application is going to the background.</summary>
    private readonly Signal<IDisposable> _onSleep = new();

    /// <summary>Signals that the application is launching fresh.</summary>
    private readonly Signal<RxVoid> _onLaunchingNew = new();

    /// <summary>Signals that the application is returning to the foreground.</summary>
    private readonly Signal<RxVoid> _onResume = new();

    /// <summary>Signals that the application is starting.</summary>
    private readonly Signal<RxVoid> _onStart = new();

    /// <summary>To detect redundant calls.</summary>
    private bool _disposedValue;

    /// <summary>Initializes static members of the <see cref="AutoSuspendHelper"/> class.</summary>
    static AutoSuspendHelper() => AppDomain.CurrentDomain.UnhandledException +=
        static (_, _) => UntimelyDemise.OnNext(RxVoid.Default);

    /// <summary>Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.</summary>
    public AutoSuspendHelper()
    {
        RxSuspension.SuspensionHost.IsLaunchingNew = _onLaunchingNew;
        RxSuspension.SuspensionHost.IsResuming = _onResume;
        RxSuspension.SuspensionHost.IsUnpausing = _onStart;
        RxSuspension.SuspensionHost.ShouldPersistState = _onSleep;
        RxSuspension.SuspensionHost.ShouldInvalidateState = UntimelyDemise;
    }

    /// <summary>Gets a subject to indicate whether the application has crashed.</summary>
    public static ISignal<RxVoid> UntimelyDemise { get; } = new Signal<RxVoid>();

    /// <summary>Call this method in the constructor of your .NET MAUI <see cref="Application" />.</summary>
    public void OnCreate() => _onLaunchingNew.OnNext(RxVoid.Default);

    /// <summary>Call this method in <see cref="Application.OnStart" /> to relay MAUI's start notification.</summary>
    public void OnStart() => _onStart.OnNext(RxVoid.Default);

    /// <summary>Call this method in <see cref="Application.OnSleep" /> when the app is going to the background.</summary>
    public void OnSleep() => _onSleep.OnNext(EmptyDisposable.Instance);

    /// <summary>Call this method in <see cref="Application.OnResume" /> when the app returns to the foreground.</summary>
    public void OnResume() => _onResume.OnNext(RxVoid.Default);

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing).
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes of the items inside the class.</summary>
    /// <param name="disposing">If we are disposing of managed objects or not.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _onLaunchingNew.Dispose();
            _onResume.Dispose();
            _onStart.Dispose();
            _onSleep.Dispose();
        }

        _disposedValue = true;
    }
}
