// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace ReactiveUI.Tests.Activation;

/// <summary>
///     Simulates an activating view fetcher.
/// </summary>
public class ActivatingViewFetcher : IActivationForViewFetcher
{
    /// <summary>
    /// The affinity value for the ActivatingView.
    /// </summary>
    private const int LargeAffinity = 100;

    /// <summary>
    ///     Gets an Observable which will activate the View.
    ///     This is called after the GetAffinityForView method.
    /// </summary>
    /// <param name="view">The view to get the activation observable for.</param>
    /// <returns>
    ///     An Observable that will return if Activation was successful.
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

    /// <summary>
    ///     Determines the priority that the Activation View Fetcher
    ///     will be able to process the view type.
    ///     0 means it cannot activate the View. Value larger than 0
    ///     indicates it can activate the View.
    ///     The class derived off IActivationForViewFetcher which returns
    ///     the highest affinity value, will be used to activate the View.
    /// </summary>
    /// <param name="view">The type for the View.</param>
    /// <returns>
    ///     The affinity value that is equal to 0 or above.
    /// </returns>
    public int GetAffinityForView(Type view) => view == typeof(ActivatingView) ? LargeAffinity : 0;
}
