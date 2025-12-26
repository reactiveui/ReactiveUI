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
    private readonly ManualResetEventSlim _shutdownEvent = new(false);
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
                try
                {
                    // Create dispatcher without showing windows
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                    App = new WpfActiveContentApp();
                    App.Startup += (s, e) => _loadComplete = true;

                    // Signal that we're ready
                    _loadComplete = true;

                    // Run the dispatcher
                    Dispatcher.Run();

                    Console.WriteLine("[WpfFixture] Dispatcher.Run() exited cleanly");
                }
                finally
                {
                    // Signal that the thread is shutting down
                    _shutdownEvent.Set();
                    Console.WriteLine("[WpfFixture] UI thread shutdown complete");
                }
            })
            {
                Name = "WPF-UIThread-Test",
                IsBackground = false // Keep as foreground to ensure proper cleanup
            };
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
            Console.WriteLine("[WpfFixture] Starting disposal...");

            if (App?.Dispatcher != null && !App.Dispatcher.HasShutdownStarted)
            {
                try
                {
                    // Request shutdown on the Dispatcher's thread
                    App.Dispatcher.InvokeShutdown();
                    Console.WriteLine("[WpfFixture] Dispatcher.InvokeShutdown() called");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WpfFixture] Error during dispatcher shutdown: {ex.Message}");
                }
            }

            // Wait for the shutdown event with a timeout
            if (_uiThread != null)
            {
                if (_shutdownEvent.Wait(TimeSpan.FromSeconds(3)))
                {
                    Console.WriteLine("[WpfFixture] UI thread shut down cleanly via shutdown event");
                }
                else if (_uiThread.Join(TimeSpan.FromSeconds(2)))
                {
                    Console.WriteLine("[WpfFixture] UI thread shut down cleanly via Join");
                }
                else
                {
                    // If thread doesn't shut down in 5 seconds total, try to abort gracefully
                    Console.WriteLine("[WpfFixture] WARNING: UI thread did not shut down within timeout");

                    // Try interrupt as last resort
                    try
                    {
                        _uiThread.Interrupt();
                        if (_uiThread.Join(TimeSpan.FromSeconds(1)))
                        {
                            Console.WriteLine("[WpfFixture] UI thread shut down after interrupt");
                        }
                        else
                        {
                            Console.WriteLine("[WpfFixture] ERROR: WPF UI thread is stuck and did not shut down");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WpfFixture] Error during thread interrupt: {ex.Message}");
                    }
                }
            }

            _shutdownEvent.Dispose();
            _disposedValue = true;
            Console.WriteLine("[WpfFixture] Disposal complete");
        }
    }
}
