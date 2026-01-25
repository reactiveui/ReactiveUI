// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

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
        typeof(ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;

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
            ? Observable.Return(false)
            : canActivate.Activated.Select(static _ => true).Merge(canActivate.Deactivated.Select(static _ => false));
}
