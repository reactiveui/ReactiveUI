// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI;

/// <summary>
/// <para>
/// ViewModelActivator is a helper class that you instantiate in your
/// ViewModel classes in order to help with Activation. Views will internally
/// call this class when the corresponding View comes on screen. This means
/// you can set up resources such as subscriptions to global objects that
/// should be cleaned up on exit. Once you instantiate this class, use the
/// WhenActivated method to register what to do when activated.
/// </para>
/// <para>
/// View Activation is  **not** the same as being loaded / unloaded; Views
/// are Activated when they *enter* the Visual Tree, and are Deactivated when
/// they *leave* the Visual Tree. This is a critical difference when it comes
/// to views that are recycled, such as UITableViews or Virtualizing
/// ScrollViews.
/// </para>
/// <para>
/// Create this class solely in the **Base Class** of any classes that inherit
/// from this class (i.e. if you create a FooViewModel that supports activation,
/// the instance should be protected and a child BarViewModel should use the
/// existing ViewModelActivator).
/// </para>
/// <para>
/// NOTE: You **must** set up Activation in the corresponding View when using
/// ViewModel Activation.
/// </para>
/// </summary>
public sealed class ViewModelActivator : IDisposable
{
    private readonly List<Func<IEnumerable<IDisposable>>> _blocks;
    private readonly Subject<Unit> _activated;
    private readonly Subject<Unit> _deactivated;
    private IDisposable _activationHandle = Disposable.Empty;
    private int _refCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelActivator"/> class.
    /// </summary>
    public ViewModelActivator()
    {
        _blocks = new List<Func<IEnumerable<IDisposable>>>();
        _activated = new Subject<Unit>();
        _deactivated = new Subject<Unit>();
    }

    /// <summary>
    /// Gets a observable which will tick every time the Activator is activated.
    /// </summary>
    /// <value>The activated.</value>
    public IObservable<Unit> Activated => _activated;

    /// <summary>
    /// Gets a observable which will tick every time the Activator is deactivated.
    /// </summary>
    /// <value>The deactivated.</value>
    public IObservable<Unit> Deactivated => _deactivated;

    /// <summary>
    /// This method is called by the framework when the corresponding View
    /// is activated. Call this method in unit tests to simulate a ViewModel
    /// being activated.
    /// </summary>
    /// <returns>A Disposable that calls Deactivate when disposed.</returns>
    public IDisposable Activate()
    {
        if (Interlocked.Increment(ref _refCount) == 1)
        {
            var disposable = new CompositeDisposable(_blocks.SelectMany(x => x()));
            Interlocked.Exchange(ref _activationHandle, disposable).Dispose();
            _activated.OnNext(Unit.Default);
        }

        return Disposable.Create(() => Deactivate());
    }

    /// <summary>
    /// This method is called by the framework when the corresponding View
    /// is deactivated.
    /// </summary>
    /// <param name="ignoreRefCount">
    /// Force the VM to be deactivated, even
    /// if more than one person called Activate.
    /// </param>
    public void Deactivate(bool ignoreRefCount = false)
    {
        if (Interlocked.Decrement(ref _refCount) == 0 || ignoreRefCount)
        {
            Interlocked.Exchange(ref _activationHandle, Disposable.Empty).Dispose();
            _deactivated.OnNext(Unit.Default);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _activationHandle.Dispose();
        _activated.Dispose();
        _deactivated.Dispose();
    }

    /// <summary>
    /// Adds a action blocks to the list of registered blocks. These will called
    /// on activation, then disposed on deactivation.
    /// </summary>
    /// <param name="block">The block to add.</param>
    internal void AddActivationBlock(Func<IEnumerable<IDisposable>> block) => _blocks.Add(block);
}