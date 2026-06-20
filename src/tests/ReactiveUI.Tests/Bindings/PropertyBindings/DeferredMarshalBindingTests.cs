// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.PropertyBindings;

/// <summary>
/// Reproduces the WPF "ViewModelToViewBindingFromBackgroundThread" scenario without WPF: a binder whose
/// ScheduleForBinding defers the change signal onto a manually-drained queue (standing in for the dispatcher marshal
/// + DoEvents pump), so the change projection runs deferred relative to the background-thread view model change.
/// </summary>
public class DeferredMarshalBindingTests
{
    /// <summary>A background-thread view-model change must reach the view once the deferred work is drained.</summary>
    /// <returns>A task.</returns>
    [Test]
    public async Task DeferredBackgroundChangePropagatesToView()
    {
        var binder = new DeferringBinder();
        var vm = new ReproViewModel();
        var view = new ReproView { ViewModel = vm };

        using var binding = binder.Bind(
            vm,
            view,
            static x => x.Text,
            static x => x.ViewText,
            (IObservable<RxVoid>?)null,
            null);

        await Task.Run(() => vm.Text = "background update");

        binder.Drain();

        await Assert.That(view.ViewText).IsEqualTo("background update");
    }

    /// <summary>A binder that defers the binding change signal onto a manual queue instead of running it inline.</summary>
    private sealed class DeferringBinder : PropertyBinderImplementation
    {
        /// <summary>The manual queue that holds deferred binding actions.</summary>
        private readonly Queue<Action> _queue = new();

        /// <summary>Runs every queued binding action in order.</summary>
        public void Drain()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
        }

        protected override IObservable<bool> ScheduleForBinding<TView>(TView view, bool value) =>
            new DeferredSignal(_queue, value);

        /// <summary>An observable that enqueues its emission onto a manual queue instead of running inline.</summary>
        /// <param name="queue">The queue that receives the deferred emission action.</param>
        /// <param name="value">The boolean value to emit when drained.</param>
        private sealed class DeferredSignal(Queue<Action> queue, bool value) : IObservable<bool>
        {
            /// <summary>Enqueues the emission of the configured value to the supplied observer.</summary>
            /// <param name="observer">The observer that receives the deferred value.</param>
            /// <returns>A disposable representing the subscription.</returns>
            public IDisposable Subscribe(IObserver<bool> observer)
            {
                queue.Enqueue(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
                return Scope.Empty;
            }
        }
    }

    /// <summary>A minimal view model exposing a single reactive text property.</summary>
    private sealed class ReproViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the reactive text value.</summary>
        public string? Text
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>A minimal view holding a reactive view model and a bound text property.</summary>
    private sealed class ReproView : ReactiveObject, IViewFor<ReproViewModel>
    {
        /// <summary>The backing field for the bound view model.</summary>
        private ReproViewModel? _viewModel;

        /// <summary>Gets or sets the value mirrored from the view model.</summary>
        public string? ViewText
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the bound view model.</summary>
        public ReproViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object? IViewFor.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (ReproViewModel?)value;
        }
    }
}
