// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// ViewModelActivator is a helper class that you instantiate in your
    /// ViewModel classes in order to help with Activation. Views will internally
    /// call this class when the corresponding View comes on screen. This means
    /// you can set up resources such as subscriptions to global objects that
    /// should be cleaned up on exit. Once you instantiate this class, use the
    /// WhenActivated method to register what to do when activated.
    ///
    /// View Activation is  **not** the same as being loaded / unloaded; Views
    /// are Activated when they *enter* the Visual Tree, and are Deactivated when
    /// they *leave* the Visual Tree. This is a critical difference when it comes
    /// to views that are recycled, such as UITableViews or Virtualizing
    /// ScrollViews.
    ///
    /// Create this class solely in the **Base Class** of any classes that inherit
    /// from this class (i.e. if you create a FooViewModel that supports activation,
    /// the instance should be protected and a child BarViewModel should use the
    /// existing ViewModelActivator).
    ///
    /// NOTE: You **must** set up Activation in the corresponding View when using
    /// ViewModel Activation.
    /// </summary>
    public sealed class ViewModelActivator
    {
        readonly List<Func<IEnumerable<IDisposable>>> blocks;
        readonly Subject<Unit> activated;
        readonly Subject<Unit> deactivated;

        IDisposable activationHandle = Disposable.Empty;
        int refCount = 0;

        /// <summary>
        /// Activated observable will tick every time the Activator is activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated { get { return activated; } }

        /// <summary>
        /// Deactivated observable will tick every time the Activator is deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        /// <summary>
        /// Constructs a new ViewModelActivator
        /// </summary>
        public ViewModelActivator()
        {
            blocks = new List<Func<IEnumerable<IDisposable>>>();
            activated = new Subject<Unit>();
            deactivated = new Subject<Unit>();
        }

        internal void addActivationBlock(Func<IEnumerable<IDisposable>> block)
        {
            blocks.Add(block);
        }

        /// <summary>
        /// This method is called by the framework when the corresponding View
        /// is activated. Call this method in unit tests to simulate a ViewModel
        /// being activated.
        /// </summary>
        /// <returns>A Disposable that calls Deactivate when disposed.</returns>
        public IDisposable Activate()
        {
            if (Interlocked.Increment(ref refCount) == 1) {
                var disp = new CompositeDisposable(blocks.SelectMany(x => x()));
                Interlocked.Exchange(ref activationHandle, disp).Dispose();
                activated.OnNext(Unit.Default);
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
            if (Interlocked.Decrement(ref refCount) == 0 || ignoreRefCount) {
                Interlocked.Exchange(ref activationHandle, Disposable.Empty).Dispose();
                deactivated.OnNext(Unit.Default);
            }
        }
    }
}
