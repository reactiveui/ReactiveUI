// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Fixture.
/// </summary>
/// <seealso cref="System.IDisposable" />
public class WpfActiveContentFixture : IDisposable
{
    private readonly Thread? _uiThread;
    private bool _loadComplete;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfActiveContentFixture"/> class.
    /// </summary>
    public WpfActiveContentFixture()
    {
        if (!_loadComplete)
        {
            _uiThread = new Thread(() =>
            {
                // Create dispatcher without showing windows
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                App = new WpfActiveContentApp();
                App.Startup += (s, e) => _loadComplete = true;

                // Don't call App.Run() - instead run a frame
                _loadComplete = true;
                Dispatcher.Run();
            });
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
            while (!_loadComplete)
            {
                Task.Delay(10).Wait();
            }
        }
    }

    /// <summary>
    /// Gets or sets the application.
    /// </summary>
    /// <value>
    /// The application.
    /// </value>
    public WpfActiveContentApp? App { get; set; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue && disposing)
        {
            if (App?.Dispatcher != null)
            {
                // Must invoke shutdown on the Dispatcher's thread
                App.Dispatcher.Invoke(() =>
                {
                    App.Dispatcher.InvokeShutdown();
                });
            }

            // Wait for UI thread to shut down with a timeout to prevent hanging
            if (_uiThread != null && !_uiThread.Join(TimeSpan.FromSeconds(5)))
            {
                // If thread doesn't shut down in 5 seconds, interrupt it
                _uiThread.Interrupt();

                // Give it one more second after interrupt
                if (!_uiThread.Join(TimeSpan.FromSeconds(1)))
                {
                    // Last resort - thread is truly stuck
                    Console.WriteLine("[WARNING] WPF UI thread did not shut down cleanly");
                }
            }

            _disposedValue = true;
        }
    }
}
