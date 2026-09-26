// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>A reactive view controller for <see cref="TestViewModel"/> that counts its activations.</summary>
public sealed class TestViewController : ReactiveViewController<TestViewModel>
{
    /// <summary>The number of times the controller has been activated.</summary>
    private int _activations;

    /// <summary>The number of times the controller has been deactivated.</summary>
    private int _deactivations;

    /// <summary>Whether the controller has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="TestViewController"/> class.</summary>
    /// <remarks>
    /// The controller passes its own view model changes to <c>WhenActivated</c>, so activation needs no reflection and
    /// the app stays trim-safe.
    /// </remarks>
    public TestViewController() =>
        this.WhenActivated(
            disposables =>
            {
                _ = Interlocked.Increment(ref _activations);
                disposables(new ActionDisposable(() => Interlocked.Increment(ref _deactivations)));
            },
            new ViewModelChanges(this));

    /// <summary>Gets the number of times the controller has been activated.</summary>
    public int Activations => Volatile.Read(ref _activations);

    /// <summary>Gets the number of times the controller has been deactivated.</summary>
    public int Deactivations => Volatile.Read(ref _deactivations);

    /// <summary>Gets a value indicating whether the controller has been disposed.</summary>
    /// <remarks>A disposed controller no longer keeps its native object alive, so a test must not send it messages.</remarks>
    public bool IsDisposed => Volatile.Read(ref _disposed);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Volatile.Write(ref _disposed, true);
        base.Dispose(disposing);
    }

    /// <summary>Emits the controller's current view model when subscribed, then every new one.</summary>
    /// <param name="view">The controller whose view model to follow.</param>
    private sealed class ViewModelChanges(TestViewController view) : IObservable<object?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<object?> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);

            observer.OnNext(view.ViewModel);
            return view.Changed
                .Where(static change => change.PropertyName == nameof(ViewModel))
                .Select(_ => (object?)view.ViewModel)
                .Subscribe(observer);
        }
    }
}
