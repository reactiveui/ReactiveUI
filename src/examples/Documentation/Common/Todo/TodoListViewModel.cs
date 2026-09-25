// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Todo;

/// <summary>
/// The view model behind the to-do screen. It loads items from an <see cref="ITodoStore"/>, filters them by text,
/// keeps a count of the items still to do and asks the view to confirm before an item is deleted.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Items = {Items.Count}, RemainingCount = {RemainingCount}")]
public sealed class TodoListViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    /// <summary>The database the view model reads and writes.</summary>
    private readonly ITodoStore _store;

    /// <summary>The subscriptions the view model owns, disposed with it.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Backs <see cref="Items"/>.</summary>
    private readonly ObservableAsPropertyHelper<IReadOnlyList<TodoItem>> _items;

    /// <summary>Backs <see cref="RemainingCount"/>.</summary>
    private readonly ObservableAsPropertyHelper<int> _remainingCount;

    /// <summary>Backs <see cref="IsLoading"/>.</summary>
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    /// <summary>Initializes a new instance of the <see cref="TodoListViewModel"/> class.</summary>
    /// <param name="store">The database to read and write.</param>
    public TodoListViewModel(ITodoStore store)
    {
        _store = store;

        Load = ReactiveCommand.CreateFromTask(_store.QueryAsync);
        _subscriptions.Add(Load.Subscribe(rows => AllItems = rows));

        var canAdd = this.WhenAnyValue(x => x.NewTitle).Select(static title => !string.IsNullOrWhiteSpace(title));
        Add = ReactiveCommand.CreateFromTask(cancellationToken => _store.AddAsync(NewTitle.Trim(), cancellationToken), canAdd);
        _subscriptions.Add(Add.Subscribe(added =>
        {
            AllItems = [.. AllItems, added];
            NewTitle = string.Empty;
            ErrorMessage = string.Empty;
        }));
        _subscriptions.Add(Add.ThrownExceptions.Subscribe(ex => ErrorMessage = ex.Message));

        Complete = ReactiveCommand.CreateFromTask<TodoItem, TodoItem>((item, cancellationToken) => _store.CompleteAsync(item.Id, cancellationToken));
        _subscriptions.Add(Complete.Subscribe(updated => AllItems = [.. AllItems.Select(item => item.Id == updated.Id ? updated : item)]));

        Delete = ReactiveCommand.CreateFromTask<TodoItem, bool>(DeleteAsync);
        _subscriptions.Add(Delete.ThrownExceptions.Subscribe(ex => ErrorMessage = ex.Message));

        _items = this.WhenAnyValue(x => x.AllItems, x => x.FilterText, Filter)
            .ToProperty(this, nameof(Items), []);
        _remainingCount = this.WhenAnyValue(x => x.AllItems)
            .Select(static all => all.Count(static item => !item.IsDone))
            .ToProperty(this, nameof(RemainingCount));
        _isLoading = Load.IsExecuting.ToProperty(this, nameof(IsLoading));

        // Reload each time the screen is shown, so the list is fresh when the user comes back to it.
        this.WhenActivated(disposables => disposables(Signal.Emit(RxVoid.Default).InvokeCommand(Load)));
    }

    /// <summary>Gets the interaction that asks the user to confirm a delete; the answer is <see langword="true"/> to go ahead.</summary>
    public Interaction<TodoItem, bool> ConfirmDelete { get; } = new();

    /// <summary>Gets the activator the view drives when the screen is shown and hidden.</summary>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets the command that reads every item from the store.</summary>
    public ReactiveCommand<RxVoid, IReadOnlyList<TodoItem>> Load { get; }

    /// <summary>Gets the command that stores <see cref="NewTitle"/> as a new item; it runs only while the title is not blank.</summary>
    public ReactiveCommand<RxVoid, TodoItem> Add { get; }

    /// <summary>Gets the command that marks an item as finished.</summary>
    public ReactiveCommand<TodoItem, TodoItem> Complete { get; }

    /// <summary>Gets the command that deletes an item once the user confirms; it returns whether the item was deleted.</summary>
    public ReactiveCommand<TodoItem, bool> Delete { get; }

    /// <summary>Gets or sets the title of the item the user is about to add.</summary>
    public string NewTitle
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the text <see cref="Items"/> is narrowed to; blank shows every item.</summary>
    public string FilterText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the message from the last refused write, or an empty string.</summary>
    public string ErrorMessage
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the items that match <see cref="FilterText"/>.</summary>
    public IReadOnlyList<TodoItem> Items => _items.Value;

    /// <summary>Gets the number of loaded items that are not finished, whatever the filter.</summary>
    public int RemainingCount => _remainingCount.Value;

    /// <summary>Gets a value indicating whether <see cref="Load"/> is running.</summary>
    public bool IsLoading => _isLoading.Value;

    /// <summary>Gets every loaded item, before the filter is applied.</summary>
    public IReadOnlyList<TodoItem> AllItems
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <inheritdoc/>
    public void Dispose()
    {
        _subscriptions.Dispose();
        _items.Dispose();
        _remainingCount.Dispose();
        _isLoading.Dispose();
        Load.Dispose();
        Add.Dispose();
        Complete.Dispose();
        Delete.Dispose();
    }

    /// <summary>Narrows a list to the items whose title contains a text.</summary>
    /// <param name="items">The items to narrow.</param>
    /// <param name="filter">The text to look for; blank keeps every item.</param>
    /// <returns>The matching items.</returns>
    private static IReadOnlyList<TodoItem> Filter(IReadOnlyList<TodoItem> items, string filter) =>
        string.IsNullOrWhiteSpace(filter)
            ? items
            : [.. items.Where(item => item.Title.Contains(filter, StringComparison.OrdinalIgnoreCase))];

    /// <summary>Asks the user to confirm, then removes the item from the store and the list.</summary>
    /// <param name="item">The item to delete.</param>
    /// <param name="cancellationToken">A token that cancels the delete.</param>
    /// <returns><see langword="true"/> when the item was deleted.</returns>
    private async Task<bool> DeleteAsync(TodoItem item, CancellationToken cancellationToken)
    {
        if (!await ConfirmDelete.Handle(item))
        {
            return false;
        }

        await _store.DeleteAsync(item.Id, cancellationToken);
        AllItems = [.. AllItems.Where(row => row.Id != item.Id)];
        return true;
    }
}
