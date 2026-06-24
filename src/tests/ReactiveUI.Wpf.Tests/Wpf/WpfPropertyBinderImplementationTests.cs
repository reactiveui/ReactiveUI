// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="WpfPropertyBinderImplementation"/>, exercising both the on-thread (inline) and off-thread (marshalled) view-update paths.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WpfPropertyBinderImplementationTests
{
    /// <summary>The value pushed from the view model before the binding is observed.</summary>
    private const string InitialValue = "initial";

    /// <summary>The value pushed from the view model on the dispatcher thread.</summary>
    private const string OnThreadValue = "on-thread update";

    /// <summary>The value pushed from the view model while off the view's dispatcher thread.</summary>
    private const string OffThreadValue = "off-thread update";

    /// <summary>The time, in milliseconds, to wait for the foreign dispatcher to start.</summary>
    private const int DispatcherStartTimeoutMs = 5000;

    /// <summary>A two-way bind whose updates originate on the view's own dispatcher thread is applied inline.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBind_OnDispatcherThread_SetsViewValueInline()
    {
        var vm = new PropertyBindViewModel { Property1 = InitialValue };
        var view = new PropertyBindView { ViewModel = vm };

        using var binding = view.Bind(view.ViewModel, static x => x.Property1, static x => x.SomeTextBox.Text);

        // Both the view and this thread share the executor's dispatcher, so CheckAccess() is true and the value is set
        // inline without marshalling.
        vm.Property1 = OnThreadValue;

        await Assert.That(view.SomeTextBox.Text).IsEqualTo(OnThreadValue);
    }

    /// <summary>
    /// A two-way bind whose view lives on a different dispatcher marshals the update through the configured main-thread
    /// scheduler instead of touching the view inline. This deterministically forces <c>CheckAccess()</c> to return
    /// false (the view belongs to a foreign dispatcher), exercising the off-thread branch on both Windows and Wine.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TwoWayBind_ViewOnForeignDispatcher_MarshalsViewValue()
    {
        using var foreign = new ForeignDispatcher();
        var vm = new PropertyBindViewModel { Property1 = InitialValue };

        // The view has thread affinity to the foreign dispatcher, so it is created and assigned its view model entirely
        // on that thread.
        var view = foreign.Invoke(() => new PropertyBindView { ViewModel = vm });

        // Route marshalled work onto the foreign dispatcher so the marshalled setter runs where the view lives.
        var previousScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = new DispatcherSequencer(foreign.Dispatcher);

        try
        {
            using var binding = foreign.Invoke(() =>
                view.Bind(view.ViewModel, static x => x.Property1, static x => x.SomeTextBox.Text));

            // This thread is NOT the view's dispatcher thread, so CheckAccess() is false and the update is scheduled.
            vm.Property1 = OffThreadValue;

            // The setter is marshalled onto the foreign dispatcher at a lower priority than a synchronous Invoke, so
            // drain the queue first; otherwise the read can run before the marshalled setter has applied.
            foreign.Flush();
            var result = foreign.Invoke(() => view.SomeTextBox.Text);

            await Assert.That(result).IsEqualTo(OffThreadValue);
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = previousScheduler;
        }
    }

    /// <summary>A dedicated STA thread that owns its own <see cref="Dispatcher"/>, used to host a view with foreign thread affinity.</summary>
    private sealed class ForeignDispatcher : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="ForeignDispatcher"/> class and starts its dispatcher loop.</summary>
        public ForeignDispatcher()
        {
            using var ready = new ManualResetEventSlim(false);
            var thread = new Thread(() =>
            {
                Dispatcher = Dispatcher.CurrentDispatcher;
                ready.Set();
                Dispatcher.Run();
            })
            {
                IsBackground = true,
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            _ = ready.Wait(DispatcherStartTimeoutMs);
        }

        /// <summary>Gets the dispatcher owned by this thread.</summary>
        public Dispatcher Dispatcher { get; private set; } = null!;

        /// <summary>Invokes the supplied function synchronously on the foreign dispatcher thread.</summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The function result.</returns>
        public T Invoke<T>(Func<T> func) => Dispatcher.Invoke(func);

        /// <summary>Drains the dispatcher queue, ensuring all queued (including marshalled) work has run.</summary>
        public void Flush() => Dispatcher.Invoke(static () => { }, DispatcherPriority.SystemIdle);

        /// <inheritdoc/>
        public void Dispose() => Dispatcher.InvokeShutdown();
    }
}
