// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Simulates a activating view fetcher.
/// </summary>
public class ActivatingViewFetcher : IActivationForViewFetcher
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
    /// <returns>
    /// The affinity value which is equal to 0 or above.
    /// </returns>
    public int GetAffinityForView(Type view) => view == typeof(ActivatingView) ? 100 : 0;

    /// <summary>
    /// Gets a Observable which will activate the View.
    /// This is called after the GetAffinityForView method.
    /// </summary>
    /// <param name="view">The view to get the activation observable for.</param>
    /// <returns>
    /// A Observable which will returns if Activation was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">The view is null.</exception>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        if (view is not ActivatingView av)
        {
            throw new ArgumentNullException(nameof(view));
        }

        return av.Loaded.Select(static _ => true).Merge(av.Unloaded.Select(static _ => false));
    }
}
