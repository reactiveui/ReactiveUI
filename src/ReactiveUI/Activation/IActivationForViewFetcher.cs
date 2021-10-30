// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI;

/// <summary>
/// Implement this interface to override how ReactiveUI determines when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
public interface IActivationForViewFetcher
{
    /// <summary>
    /// Determines the priority that the Activation View Fetcher
    /// will be able to process the view type.
    /// 0 means it cannot activate the View, value larger than 0
    /// indicates it can activate the View.
    /// The class derived off IActivationForViewFetcher which returns
    /// the highest affinity value will be used to activate the View.
    /// </summary>
    /// <param name="view">The type for the View.</param>
    /// <returns>The affinity value which is equal to 0 or above.</returns>
    int GetAffinityForView(Type view);

    /// <summary>
    /// Gets a Observable which will activate the View.
    /// This is called after the GetAffinityForView method.
    /// </summary>
    /// <param name="view">The view to get the activation observable for.</param>
    /// <returns>A Observable which will returns if Activation was successful.</returns>
    IObservable<bool> GetActivationForView(IActivatableView view);
}