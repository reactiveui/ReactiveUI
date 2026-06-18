// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
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
    /// <summary>List of registered activation blocks run when the view model is activated.</summary>
    private readonly List<Func<IEnumerable<IDisposable>>> _blocks;

    /// <summary>Subject that emits each time the view model is activated.</summary>
    private readonly Signal<RxVoid> _activated;

    /// <summary>Subject that emits each time the view model is deactivated.</summary>
    private readonly Signal<RxVoid> _deactivated;

    /// <summary>Cached deactivation callback so each <see cref="Activate"/> doesn't allocate a fresh method-group delegate.</summary>
    private readonly Action _deactivate;

    /// <summary>Composite disposable that is replaced on each activation cycle.</summary>
    private IDisposable _activationHandle = EmptyDisposable.Instance;

    /// <summary>Reference count tracking how many times Activate has been called without a matching Deactivate.</summary>
    private int _refCount;

    /// <summary>Initializes a new instance of the <see cref="ViewModelActivator"/> class.</summary>
    public ViewModelActivator()
    {
        _blocks = [];
        _activated = new();
        _deactivated = new();
        _deactivate = Deactivate;
    }

    /// <summary>Gets a observable which will tick every time the Activator is activated.</summary>
    /// <value>The activated.</value>
    public IObservable<RxVoid> Activated => _activated;

    /// <summary>Gets a observable which will tick every time the Activator is deactivated.</summary>
    /// <value>The deactivated.</value>
    public IObservable<RxVoid> Deactivated => _deactivated;

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
            // Build the composite with a plain loop rather than SelectMany so activation doesn't allocate a LINQ
            // iterator plus an intermediate buffer on every cycle.
            var disposable = new MultipleDisposable();
            foreach (var block in _blocks)
            {
                foreach (var item in block())
                {
                    disposable.Add(item);
                }
            }

            Interlocked.Exchange(ref _activationHandle, disposable).Dispose();
            _activated.OnNext(RxVoid.Default);
        }

        return new ActionDisposable(_deactivate);
    }

    /// <summary>
    /// This method is called by the framework when the corresponding View
    /// is deactivated. Respects the activation reference count.
    /// </summary>
    public void Deactivate() => Deactivate(false);

    /// <summary>This method is called by the framework when the corresponding View is deactivated.</summary>
    /// <param name="ignoreRefCount">
    /// Force the VM to be deactivated, even
    /// if more than one person called Activate.
    /// </param>
    public void Deactivate(bool ignoreRefCount)
    {
        if (ignoreRefCount)
        {
            Interlocked.Exchange(ref _refCount, 0);
            Interlocked.Exchange(ref _activationHandle, EmptyDisposable.Instance).Dispose();
            _deactivated.OnNext(RxVoid.Default);
            return;
        }

        int current;
        int next;
        do
        {
            current = Volatile.Read(ref _refCount);
            if (current <= 0)
            {
                return;
            }

            next = current - 1;
        }
        while (Interlocked.CompareExchange(ref _refCount, next, current) != current);

        if (next != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _activationHandle, EmptyDisposable.Instance).Dispose();
        _deactivated.OnNext(RxVoid.Default);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _activationHandle.Dispose();
        _activated.Dispose();
        _deactivated.Dispose();
    }

    /// <summary>Adds a action blocks to the list of registered blocks. These will called on activation, then disposed on deactivation.</summary>
    /// <param name="block">The block to add.</param>
    internal void AddActivationBlock(Func<IEnumerable<IDisposable>> block) => _blocks.Add(block);
}
