// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;

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
            (IObservable<System.Reactive.Unit>?)null,
            null);

        await Task.Run(() => vm.Text = "background update");

        binder.Drain();

        await Assert.That(view.ViewText).IsEqualTo("background update");
    }

    /// <summary>A binder that defers the binding change signal onto a manual queue instead of running it inline.</summary>
    private sealed class DeferringBinder : PropertyBinderImplementation
    {
        private readonly Queue<Action> _queue = new();

        public void Drain()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
        }

        protected override IObservable<bool> ScheduleForBinding<TView>(TView view, bool value) =>
            new DeferredSignal(_queue, value);

        private sealed class DeferredSignal(Queue<Action> queue, bool value) : IObservable<bool>
        {
            public IDisposable Subscribe(IObserver<bool> observer)
            {
                queue.Enqueue(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
                return Disposable.Empty;
            }
        }
    }

    private sealed class ReproViewModel : ReactiveObject
    {
        private string? _text;

        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }

    private sealed class ReproView : ReactiveObject, IViewFor<ReproViewModel>
    {
        private string? _viewText;
        private ReproViewModel? _viewModel;

        public string? ViewText
        {
            get => _viewText;
            set => this.RaiseAndSetIfChanged(ref _viewText, value);
        }

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
