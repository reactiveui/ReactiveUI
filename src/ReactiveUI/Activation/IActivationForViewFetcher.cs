// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Implement this interface to override how ReactiveUI determines when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
/// <remarks>
/// <para>
/// Activation fetchers translate framework-specific signals (such as page navigation, focus, or visibility
/// changes) into the cross-platform <see cref="IActivatableView"/> semantics used by ReactiveUI. Multiple
/// fetchers can exist, each advertising an affinity for a given view type.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public sealed class WinFormsActivationFetcher : IActivationForViewFetcher
/// {
///     public int GetAffinityForView(Type view) => typeof(Form).IsAssignableFrom(view) ? 10 : 0;
///
///     public IObservable<bool> GetActivationForView(IActivatableView view)
///     {
///         var form = (Form)view;
///         return Observable.Merge(
///             Observable.FromEventPattern(form, nameof(form.Load)).Select(_ => true),
///             Observable.FromEventPattern(form, nameof(form.FormClosed)).Select(_ => false));
///     }
/// }
/// ]]>
/// </code>
/// </example>
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
