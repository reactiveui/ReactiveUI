// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Device.Tests;

/// <summary>
/// Tests that setting the view model through <see cref="IViewFor.ViewModel"/> on each Android reactive activity and
/// fragment raises the same change as setting the typed property.
/// </summary>
public class ViewForViewModelTests
{
    /// <summary>A <see cref="ReactiveActivity{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactiveActivity_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new ActivatingActivity());

    /// <summary>A platform <see cref="ReactiveFragment{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactiveFragment_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new PlainPlatformFragment());

    /// <summary>An AndroidX <see cref="AndroidX.ReactiveAppCompatActivity{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactiveAppCompatActivity_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new CompatHostActivity());

    /// <summary>An AndroidX <see cref="AndroidX.ReactiveFragmentActivity{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactiveFragmentActivity_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new PlainFragmentActivity());

    /// <summary>An AndroidX <see cref="AndroidX.ReactiveFragment{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task AndroidXReactiveFragment_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new WiredFragment());

    /// <summary>An AndroidX <see cref="AndroidX.ReactiveDialogFragment{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactiveDialogFragment_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new PlainDialogFragment());

    /// <summary>An AndroidX <see cref="AndroidX.ReactivePreferenceFragment{TViewModel}"/> raises the change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public Task ReactivePreferenceFragment_SettingViewModelThroughIViewFor_RaisesPropertyChanged() =>
        AssertRaisesViewModelChanged(static () => new PlainPreferenceFragment());

    /// <summary>
    /// Creates a view on the main thread, sets its view model through <see cref="IViewFor"/>, and checks that the
    /// typed property holds it and that <see cref="INotifyPropertyChanged.PropertyChanged"/> named it.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="create">Creates the view.</param>
    /// <returns>A task representing the check.</returns>
    private static async Task AssertRaisesViewModelChanged<TView>(Func<TView> create)
        where TView : IViewFor<TestViewModel>, INotifyPropertyChanged
    {
        var viewModel = new TestViewModel("untyped");

        var (typed, changed) = await MainThread.RunAsync(() =>
        {
            var view = create();
            List<string?> names = [];
            view.PropertyChanged += (_, e) => names.Add(e.PropertyName);

            ((IViewFor)view).ViewModel = viewModel;

            return (view.ViewModel, names);
        });

        await Assert.That(typed).IsSameReferenceAs(viewModel);
        await Assert.That(changed).Contains(nameof(IViewFor.ViewModel));
    }
}
