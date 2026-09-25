// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;

namespace ReactiveUI.Documentation.Todo;

/// <summary>
/// The to-do screen: a title box and add button, a filter box, the list and a count of what is left. While the screen is
/// shown it binds its controls to the view model, and it drops those bindings when it is hidden.
/// </summary>
[System.Diagnostics.DebuggerDisplay("TodoListView ViewModel = {ViewModel}")]
public sealed class TodoListView : ReactiveObject, IViewFor<TodoListViewModel>, ICanActivate, IDisposable
{
    /// <summary>Raised when the screen is shown.</summary>
    private readonly Signal<RxVoid> _activated = new();

    /// <summary>Raised when the screen is hidden.</summary>
    private readonly Signal<RxVoid> _deactivated = new();

    /// <summary>Initializes a new instance of the <see cref="TodoListView"/> class.</summary>
    public TodoListView() =>
        this.WhenActivated(disposables =>
        {
            disposables(this.Bind(ViewModel, x => x.NewTitle, v => v.NewTitleBox.Text));
            disposables(this.Bind(ViewModel, x => x.FilterText, v => v.FilterBox.Text));
            disposables(this.OneWayBind(ViewModel, x => x.Items, v => v.ItemList.Items));
            disposables(this.OneWayBind(ViewModel, x => x.RemainingCount, v => v.RemainingLabel.Text, static count => $"{count} left"));
            disposables(this.OneWayBind(ViewModel, x => x.ErrorMessage, v => v.ErrorLabel.Text));
            disposables(this.BindCommand(ViewModel, x => x.Add, v => v.AddButton));
        });

    /// <summary>Gets the box the user types a new title into.</summary>
    public TextBox NewTitleBox { get; } = new();

    /// <summary>Gets the button that adds the typed title.</summary>
    public Button AddButton { get; } = new();

    /// <summary>Gets the box the user types a filter into.</summary>
    public TextBox FilterBox { get; } = new();

    /// <summary>Gets the list of items.</summary>
    public ListBox<TodoItem> ItemList { get; } = new();

    /// <summary>Gets the button that finishes the selected item.</summary>
    public Button CompleteButton { get; } = new();

    /// <summary>Gets the label that shows how many items are left.</summary>
    public Label RemainingLabel { get; } = new();

    /// <summary>Gets the label that shows the last error.</summary>
    public Label ErrorLabel { get; } = new();

    /// <summary>Gets or sets the view model the screen shows.</summary>
    public TodoListViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TodoListViewModel?)value;
    }

    /// <inheritdoc/>
    public IObservable<RxVoid> Activated => _activated;

    /// <inheritdoc/>
    public IObservable<RxVoid> Deactivated => _deactivated;

    /// <summary>Shows the screen, as a window or page does when it is navigated to.</summary>
    public void Show() => _activated.OnNext(RxVoid.Default);

    /// <summary>Hides the screen, as a window or page does when it is navigated away from.</summary>
    public void Hide() => _deactivated.OnNext(RxVoid.Default);

    /// <inheritdoc/>
    public void Dispose()
    {
        _activated.Dispose();
        _deactivated.Dispose();
    }
}
