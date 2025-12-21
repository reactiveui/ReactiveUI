// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui;

/// <summary>
/// Helps manage .NET MAUI application lifecycle events.
/// </summary>
/// <remarks>
/// <para>
/// Instantiate this class to wire <see cref="RxApp.SuspensionHost"/> to MAUI's <see cref="Microsoft.Maui.Controls.Application"/>
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
///
///   public App()
///   {
///     _autoSuspendHelper = new AutoSuspendHelper();
///     RxApp.SuspensionHost.CreateNewAppState = () => new MainState();
///     RxApp.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(FileSystem.AppDataDirectory));
///     _autoSuspendHelper.OnCreate();
///
///     InitializeComponent();
///     MainPage = new MainView();
///   }
///
///   protected override void OnStart() => _autoSuspendHelper.OnStart();
///
///   protected override void OnResume() => _autoSuspendHelper.OnResume();
///
///   protected override void OnSleep() => _autoSuspendHelper.OnSleep();
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("AutoSuspendHelper uses RxApp.SuspensionHost which requires dynamic code generation")]
[RequiresUnreferencedCode("AutoSuspendHelper uses RxApp.SuspensionHost which may require unreferenced code")]
#endif
public partial class AutoSuspendHelper : IEnableLogger, IDisposable
{
    private readonly Subject<IDisposable> _onSleep = new();
    private readonly Subject<Unit> _onLaunchingNew = new();
    private readonly Subject<Unit> _onResume = new();
    private readonly Subject<Unit> _onStart = new();
    private bool _disposedValue; // To detect redundant calls

    /// <summary>
    /// Initializes static members of the <see cref="AutoSuspendHelper"/> class.
    /// </summary>
    static AutoSuspendHelper() => AppDomain.CurrentDomain.UnhandledException += static (_, _) => UntimelyDemise.OnNext(Unit.Default);

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
    /// </summary>
    public AutoSuspendHelper()
    {
        RxApp.SuspensionHost.IsLaunchingNew = _onLaunchingNew;
        RxApp.SuspensionHost.IsResuming = _onResume;
        RxApp.SuspensionHost.IsUnpausing = _onStart;
        RxApp.SuspensionHost.ShouldPersistState = _onSleep;
        RxApp.SuspensionHost.ShouldInvalidateState = UntimelyDemise;
    }

    /// <summary>
    /// Gets a subject to indicate whether the application has crashed.
    /// </summary>
    public static Subject<Unit> UntimelyDemise { get; } = new();

    /// <summary>
    /// Call this method in the constructor of your .NET MAUI
    /// <see cref="Microsoft.Maui.Controls.Application" />.
    /// </summary>
    public void OnCreate() => _onLaunchingNew.OnNext(Unit.Default);

    /// <summary>
    /// Call this method in <see cref="Microsoft.Maui.Controls.Application.OnStart" /> to relay MAUI's start notification.
    /// </summary>
    public void OnStart() => _onStart.OnNext(Unit.Default);

    /// <summary>
    /// Call this method in <see cref="Microsoft.Maui.Controls.Application.OnSleep" /> when the app is going to the background.
    /// </summary>
    public void OnSleep() => _onSleep.OnNext(Disposable.Empty);

    /// <summary>
    /// Call this method in <see cref="Microsoft.Maui.Controls.Application.OnResume" /> when the app returns to the foreground.
    /// </summary>
    public void OnResume() => _onResume.OnNext(Unit.Default);

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing).
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the items inside the class.
    /// </summary>
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
