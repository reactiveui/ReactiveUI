// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
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
                App = new WpfActiveContentApp();
                App.Startup += (s, e) => _loadComplete = true;
                App.Run();
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
            App?.Dispatcher.Invoke(
                () => App.Shutdown(),
                DispatcherPriority.Normal);
            Thread.Sleep(500);
            _uiThread?.Join();
            _disposedValue = true;
        }
    }
}
