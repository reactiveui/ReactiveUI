// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reflection;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// This class implements View Activation for classes that explicitly describe
/// their activation via <see cref="ICanActivate"/>. This class is used by the framework.
/// </summary>
public class CanActivateViewFetcher : IActivationForViewFetcher
{
    /// <summary>
    /// Determines the affinity score for the specified view type based on whether it implements the ICanActivate
    /// interface.
    /// </summary>
    /// <remarks>Use this method to assess whether a view type is suitable for activation scenarios that
    /// require the ICanActivate interface. A higher affinity score indicates a stronger match.</remarks>
    /// <param name="view">The type of the view to evaluate for activation capability. Cannot be null.</param>
    /// <returns>An integer affinity score: 10 if the view type implements ICanActivate; otherwise, 0.</returns>
    public int GetAffinityForView(Type view) =>
        typeof(ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? BindingAffinity.ExactType : 0;

    /// <summary>
    /// Returns an observable sequence that indicates the activation state of the specified view.
    /// </summary>
    /// <remarks>If the provided view does not implement <see cref="ICanActivate"/>, the returned observable
    /// emits <see langword="false"/> and completes immediately. Otherwise, the observable reflects the view's
    /// activation and deactivation events as they occur.</remarks>
    /// <param name="view">The view for which to observe activation and deactivation events. If the view does not support activation, the
    /// observable will emit a single value of <see langword="false"/>.</param>
    /// <returns>An observable sequence of <see langword="true"/> and <see langword="false"/> values that reflect the activation
    /// and deactivation state of the view. The sequence emits <see langword="true"/> when the view is activated and
    /// <see langword="false"/> when it is deactivated.</returns>
    public IObservable<bool> GetActivationForView(IActivatableView view) =>
        view is not ICanActivate canActivate
            ? SingleValueObservable.False
            : new ActivationStateObservable(canActivate.Activated, canActivate.Deactivated);

    /// <summary>
    /// A fused sink that emits <see langword="true"/> on each <see cref="ICanActivate.Activated"/> tick and
    /// <see langword="false"/> on each <see cref="ICanActivate.Deactivated"/> tick — replacing
    /// <c>Activated.Select(_ =&gt; true).Merge(Deactivated.Select(_ =&gt; false))</c>.
    /// </summary>
    /// <param name="activated">Emits when the view is activated.</param>
    /// <param name="deactivated">Emits when the view is deactivated.</param>
    private sealed class ActivationStateObservable(IObservable<Unit> activated, IObservable<Unit> deactivated)
        : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer);
            sink.Run(activated, deactivated);
            return sink;
        }

        /// <summary>Forwards a constant value for each tick of one source, completing only when both sources complete.</summary>
        private sealed class Sink(IObserver<bool> downstream) : IDisposable
        {
            /// <summary>The number of sources that must complete before downstream completes.</summary>
            private const int SourceCount = 2;

            /// <summary>Serializes notifications from the two sources and guards the completion count.</summary>
            #if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
            #else
            private readonly object _gate = new();
            #endif

            /// <summary>The activated-source subscription.</summary>
            private readonly OnceDisposable _activated = new();

            /// <summary>The deactivated-source subscription.</summary>
            private readonly OnceDisposable _deactivated = new();

            /// <summary>The number of sources that have completed; downstream completes when both have.</summary>
            private int _completed;

            /// <summary>Whether this sink has been disposed or terminated.</summary>
            private bool _done;

            /// <summary>Begins observing both sources.</summary>
            /// <param name="activated">Emits when the view is activated.</param>
            /// <param name="deactivated">Emits when the view is deactivated.</param>
            public void Run(IObservable<Unit> activated, IObservable<Unit> deactivated)
            {
                _activated.Disposable = activated.Subscribe(new BranchObserver(this, true));
                _deactivated.Disposable = deactivated.Subscribe(new BranchObserver(this, false));
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                lock (_gate)
                {
                    if (_done)
                    {
                        return;
                    }

                    _done = true;
                }

                _activated.Dispose();
                _deactivated.Dispose();
            }

            /// <summary>Forwards this branch's constant value to the downstream observer.</summary>
            /// <param name="value">The constant value for the branch that ticked.</param>
            private void OnBranchNext(bool value)
            {
                lock (_gate)
                {
                    if (_done)
                    {
                        return;
                    }

                    downstream.OnNext(value);
                }
            }

            /// <summary>Terminates with the error from either branch and disposes both subscriptions.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnBranchError(Exception error)
            {
                lock (_gate)
                {
                    if (_done)
                    {
                        return;
                    }

                    _done = true;
                    downstream.OnError(error);
                }

                _activated.Dispose();
                _deactivated.Dispose();
            }

            /// <summary>Completes downstream once both branches have completed.</summary>
            private void OnBranchCompleted()
            {
                lock (_gate)
                {
                    if (_done || ++_completed < SourceCount)
                    {
                        return;
                    }

                    _done = true;
                    downstream.OnCompleted();
                }
            }

            /// <summary>Maps each tick of one source to a constant boolean and forwards it to the parent sink.</summary>
            private sealed class BranchObserver(Sink parent, bool value) : IObserver<Unit>
            {
                /// <inheritdoc/>
                public void OnNext(Unit unit) => parent.OnBranchNext(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnBranchError(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnBranchCompleted();
            }
        }
    }
}
