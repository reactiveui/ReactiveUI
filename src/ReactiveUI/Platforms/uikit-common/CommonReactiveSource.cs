// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using DynamicData;
using DynamicData.Binding;

using Foundation;

namespace ReactiveUI;

/// <summary>
/// Provides a common reactive data source implementation for collection-based UI views backed by sectioned data.
/// </summary>
/// <typeparam name="TSource">The source item type.</typeparam>
/// <typeparam name="TUIView">The UI view type.</typeparam>
/// <typeparam name="TUIViewCell">The UI cell type.</typeparam>
/// <typeparam name="TSectionInfo">The section information type.</typeparam>
/// <remarks>
/// <para>
/// This type monitors the current <see cref="SectionInfo"/> and the collections inside each section, and translates
/// collection change notifications into batch updates for the underlying UI adapter.
/// </para>
/// <para>
/// Threading: all operations are expected to run on the creating thread (typically the platform main thread). If an
/// operation occurs off-thread, an <see cref="InvalidOperationException"/> is thrown.
/// </para>
/// <para>
/// Trimming/AOT: this implementation avoids expression-tree-based reactive helpers (e.g. WhenAnyValue/ObservableForProperty)
/// and instead filters ReactiveObject change streams by property name.
/// </para>
/// </remarks>
internal sealed class CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable
    where TSectionInfo : ISectionInformation<TUIViewCell>
{
    /// <summary>
    /// Adapter used to manipulate the UI view (reload, begin/end updates, insert/delete items, etc.).
    /// </summary>
    private readonly IUICollViewAdapter<TUIView, TUIViewCell> _adapter;

    /// <summary>
    /// Managed thread id captured at construction time; used to validate calls occur on the expected thread.
    /// </summary>
    private readonly int _mainThreadId;

    /// <summary>
    /// Root disposable for subscriptions created by this instance.
    /// </summary>
    private readonly CompositeDisposable _mainDisposables;

    /// <summary>
    /// Holds subscriptions associated with the current <see cref="SectionInfo"/> value. Replaced when SectionInfo changes.
    /// </summary>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Handled by the CompositeDisposable")]
    private readonly SerialDisposable _sectionInfoDisposable;

    /// <summary>
    /// Pending collection changes captured while the UI is not reloading and before a scheduled batch update is applied.
    /// </summary>
    private readonly List<(int section, PendingChange pendingChange)> _pendingChanges;

    /// <summary>
    /// Indicates whether pending changes are currently being collected for later application.
    /// </summary>
    private bool _isCollectingChanges;

    /// <summary>
    /// Backing store for <see cref="SectionInfo"/>.
    /// </summary>
    private IReadOnlyList<TSectionInfo> _sectionInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonReactiveSource{TSource, TUIView, TUIViewCell, TSectionInfo}"/> class.
    /// </summary>
    /// <param name="adapter">The adapter to use which we want to display information for.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="adapter"/> is <see langword="null"/>.</exception>
    public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter)
    {
        ArgumentExceptionHelper.ThrowIfNull(adapter);

        _adapter = adapter;
        _mainThreadId = Environment.CurrentManagedThreadId;

        _mainDisposables = [];
        _sectionInfoDisposable = new SerialDisposable();
        _mainDisposables.Add(_sectionInfoDisposable);

        _pendingChanges = [];
        _sectionInfo = [];

        // Avoid ObservableForProperty/WhenAnyValue (expression trees); filter by property name instead.
        _mainDisposables.Add(
            Changing!
                .Where(static e => e.PropertyName == nameof(SectionInfo))
                .Subscribe(
                    _ => SectionInfoChanging(),
                    ex => this.Log().Error(ex, "Error occurred whilst SectionInfo changing.")));

        _mainDisposables.Add(
            Changed!
                .Where(static e => e.PropertyName == nameof(SectionInfo))
                .Subscribe(
                    _ => SectionInfoChanged(SectionInfo),
                    ex => this.Log().Error(ex, "Error occurred when SectionInfo changed.")));
    }

    /// <summary>
    /// Gets or sets the current section information.
    /// </summary>
    /// <remarks>
    /// Assigning this property replaces all subscriptions associated with the prior section information.
    /// </remarks>
    public IReadOnlyList<TSectionInfo> SectionInfo
    {
        get => _sectionInfo;
        set => this.RaiseAndSetIfChanged(ref _sectionInfo, value);
    }

    /// <summary>
    /// Gets a value indicating whether debug logging is enabled.
    /// </summary>
    private bool IsDebugEnabled => this.Log().Level <= LogLevel.Debug;

    /// <summary>
    /// Returns the number of sections.
    /// </summary>
    /// <returns>The number of sections.</returns>
    public int NumberOfSections()
    {
        VerifyOnMainThread();

        var count = _sectionInfo.Count;
        this.Log().Debug(CultureInfo.InvariantCulture, "Reporting number of sections = {0}", count);

        return count;
    }

    /// <summary>
    /// Returns the number of rows in a section.
    /// </summary>
    /// <param name="section">The section index.</param>
    /// <returns>The number of rows in the specified section.</returns>
    public int RowsInSection(int section)
    {
        VerifyOnMainThread();

        var list = (IList)_sectionInfo[section].Collection!;
        var count = list.Count;

        this.Log().Debug(CultureInfo.InvariantCulture, "Reporting rows in section {0} = {1}", section, count);

        return count;
    }

    /// <summary>
    /// Returns the item at the specified index path.
    /// </summary>
    /// <param name="path">The index path.</param>
    /// <returns>The item at the index path, or <see langword="null"/>.</returns>
    public object? ItemAt(NSIndexPath path)
    {
        VerifyOnMainThread();

        var list = (IList)_sectionInfo[path.Section].Collection!;
        this.Log().Debug(CultureInfo.InvariantCulture, "Returning item at {0}-{1}", path.Section, path.Row);

        return list[path.Row];
    }

    /// <summary>
    /// Dequeues and configures a cell for the specified index path.
    /// </summary>
    /// <param name="indexPath">The index path.</param>
    /// <returns>The configured view cell.</returns>
    public TUIViewCell GetCell(NSIndexPath indexPath)
    {
        VerifyOnMainThread();

        this.Log().Debug(CultureInfo.InvariantCulture, "Getting cell for index path {0}-{1}", indexPath.Section, indexPath.Row);

        var section = _sectionInfo[indexPath.Section];
        var vm = ((IList)section.Collection!)[indexPath.Row];

        var key = section?.CellKeySelector?.Invoke(vm) ?? NSString.Empty;
        var cell = _adapter.DequeueReusableCell(key, indexPath);

        if (cell is IViewFor view)
        {
            this.Log().Debug(CultureInfo.InvariantCulture, "Setting VM for index path {0}-{1}", indexPath.Section, indexPath.Row);
            view.ViewModel = vm;
        }

        var initializeCellAction = section?.InitializeCellAction ?? NoOpInitializeCell;
        initializeCellAction(cell);

        return cell;
    }

    /// <summary>
    /// Disposes subscriptions and managed resources associated with this instance.
    /// </summary>
    public void Dispose()
    {
        _mainDisposables.Dispose();
    }

    /// <summary>
    /// No-op initializer used when a section does not provide an initialization callback.
    /// </summary>
    /// <param name="cell">The cell to initialize.</param>
    private static void NoOpInitializeCell(TUIViewCell cell)
    {
    }

    /// <summary>
    /// Builds the list of updates for a pending collection change event.
    /// </summary>
    /// <param name="pendingChange">The pending change.</param>
    /// <returns>An enumerable of updates.</returns>
    /// <exception cref="NotSupportedException">Thrown when the action is not supported.</exception>
    private static IEnumerable<Update> GetUpdatesForEvent(PendingChange pendingChange) =>
        pendingChange.Action switch
        {
            NotifyCollectionChangedAction.Add =>
                Enumerable
                    .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                    .Select(Update.CreateAdd),

            NotifyCollectionChangedAction.Remove =>
                Enumerable
                    .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
                    .Select(_ => Update.CreateDelete(pendingChange.OldStartingIndex)),

            // Use OldStartingIndex for each "Update.Index" because the batch update processes and removes items sequentially
            // opposed to as one Range operation.
            NotifyCollectionChangedAction.Move =>
                Enumerable
                    .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
                    .Select(Update.CreateDelete)
                    .Concat(
                        Enumerable
                            .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                            .Select(Update.CreateAdd)),

            NotifyCollectionChangedAction.Replace =>
                Enumerable
                    .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                    .SelectMany(static x => new[] { Update.CreateDelete(x), Update.CreateAdd(x) }),

            _ => throw new NotSupportedException("Don't know how to deal with " + pendingChange.Action),
        };

    /// <summary>
    /// Called before <see cref="SectionInfo"/> changes. Disposes subscriptions for the current SectionInfo.
    /// </summary>
    private void SectionInfoChanging()
    {
        VerifyOnMainThread();

        this.Log().Debug(CultureInfo.InvariantCulture, "SectionInfo about to change, disposing of any subscriptions for previous SectionInfo.");
        _sectionInfoDisposable.Disposable = null;
    }

    /// <summary>
    /// Called when <see cref="SectionInfo"/> changes. Subscribes to section structure and item changes.
    /// </summary>
    /// <param name="sectionInfo">The new section info.</param>
    private void SectionInfoChanged(IReadOnlyList<TSectionInfo>? sectionInfo)
    {
        VerifyOnMainThread();

        // this ID just makes it possible to correlate log messages with a specific SectionInfo
        var sectionInfoId = SectionInfoIdGenerator.Generate();
        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] SectionInfo changed to {1}.", sectionInfoId, sectionInfo);

        if (sectionInfo is null)
        {
            _sectionInfoDisposable.Disposable = null;
            return;
        }

        var notifyCollectionChanged = sectionInfo as INotifyCollectionChanged;
        if (notifyCollectionChanged is null)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "[#{0}] SectionInfo {1} does not implement INotifyCollectionChanged - any added or removed sections will not be reflected in the UI.",
                sectionInfoId,
                sectionInfo);
        }

        var sectionChanged =
            (notifyCollectionChanged is null
                ? Observable<Unit>.Never
                : notifyCollectionChanged.ObserveCollectionChanges().Select(static _ => Unit.Default))
            .StartWith(Unit.Default);

        var disposables = new CompositeDisposable
        {
            Disposable.Create(() => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Disposed of section info", sectionInfoId))
        };

        _sectionInfoDisposable.Disposable = disposables;
        SubscribeToSectionInfoChanges(sectionInfoId, sectionInfo, sectionChanged, disposables);
    }

    /// <summary>
    /// Subscribes to changes in the section collection and to changes within each section's item collection.
    /// </summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="sectionInfo">The current section info.</param>
    /// <param name="sectionChanged">An observable indicating that the section set changed.</param>
    /// <param name="disposables">A disposable container for subscriptions.</param>
    private void SubscribeToSectionInfoChanges(int sectionInfoId, IReadOnlyList<TSectionInfo> sectionInfo, IObservable<Unit> sectionChanged, CompositeDisposable disposables)
    {
        // holds a single disposable representing the monitoring of sectionInfo.
        // once disposed, any changes to sectionInfo will no longer trigger any of the logic below
        var sectionInfoDisposable = new SerialDisposable();
        disposables.Add(sectionInfoDisposable);

        disposables.Add(
            sectionChanged.Subscribe(
                _ =>
                {
                    VerifyOnMainThread();

                    this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Calling ReloadData()", sectionInfoId);
                    _adapter.ReloadData();

                    // holds all the disposables created to monitor stuff inside the section
                    var sectionDisposables = new CompositeDisposable();
                    sectionInfoDisposable.Disposable = sectionDisposables;

                    // holds a single disposable for any outstanding request to apply pending changes
                    var applyPendingChangesDisposable = new SerialDisposable();
                    sectionDisposables.Add(applyPendingChangesDisposable);

                    var isReloading = _adapter
                        .IsReloadingData
                        .DistinctUntilChanged()
                        .Do(y => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] IsReloadingData = {1}", sectionInfoId, y))
                        .Publish();

                    // Merge per-section collection changes into a single stream.
                    // This remains a "setup path" rather than a hot per-event path.
                    var anySectionChanged = sectionInfo
                        .Select((y, index) => y.Collection!.ObserveCollectionChanges().Select(z => new { Section = index, Change = z }))
                        .Merge()
                        .Publish();

                    // since reloads are applied asynchronously, it is possible for data to change whilst the reload is occurring
                    // thus, we need to ensure any such changes result in another reload
                    sectionDisposables.Add(
                        isReloading
                            .Where(static y => y)
                            .Join(
                                anySectionChanged,
                                _ => isReloading,
                                _ => Observable<Unit>.Empty,
                                static (_, __) => Unit.Default)
                            .Subscribe(
                                _ =>
                                {
                                    VerifyOnMainThread();

                                    this.Log().Debug(
                                        CultureInfo.InvariantCulture,
                                        "[#{0}] A section changed whilst a reload is in progress - forcing another reload.",
                                        sectionInfoId);

                                    _adapter.ReloadData();
                                    _pendingChanges.Clear();
                                    _isCollectingChanges = false;
                                }));

                    sectionDisposables.Add(
                        isReloading
                            .Where(static y => !y)
                            .Join(
                                anySectionChanged,
                                _ => isReloading,
                                _ => Observable<Unit>.Empty,
                                static (_, changeDetails) => (changeDetails.Change, changeDetails.Section))
                            .Subscribe(
                                y =>
                                {
                                    VerifyOnMainThread();

                                    if (IsDebugEnabled)
                                    {
                                        var ea = y.Change.EventArgs;
                                        this.Log().Debug(
                                            CultureInfo.InvariantCulture,
                                            "[#{0}] Change detected in section {1} : Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                            sectionInfoId,
                                            y.Section,
                                            ea.Action,
                                            ea.OldStartingIndex,
                                            ea.NewStartingIndex,
                                            ea.OldItems is null ? "null" : ea.OldItems.Count.ToString(CultureInfo.InvariantCulture),
                                            ea.NewItems is null ? "null" : ea.NewItems.Count.ToString(CultureInfo.InvariantCulture));
                                    }

                                    if (!_isCollectingChanges)
                                    {
                                        this.Log().Debug(
                                            CultureInfo.InvariantCulture,
                                            "[#{0}] A section changed whilst no reload is in progress - instigating collection of updates for later application.",
                                            sectionInfoId);

                                        _isCollectingChanges = true;

                                        // immediately indicate to the view that there are changes underway, even though we don't apply them immediately
                                        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] BeginUpdates", sectionInfoId);
                                        _adapter.BeginUpdates();

                                        applyPendingChangesDisposable.Disposable =
                                            RxSchedulers.MainThreadScheduler.Schedule(
                                                () =>
                                                {
                                                    ApplyPendingChanges(sectionInfoId);

                                                    this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] EndUpdates", sectionInfoId);
                                                    _adapter.EndUpdates();

                                                    _isCollectingChanges = false;
                                                    applyPendingChangesDisposable.Disposable = null;
                                                });
                                    }

                                    _pendingChanges.Add((y.Section, new PendingChange(y.Change.EventArgs)));
                                },
                                ex => this.Log().Error(CultureInfo.InvariantCulture, "[#{0}] Error while watching section collection: {1}", sectionInfoId, ex)));

                    sectionDisposables.Add(isReloading.Connect());
                    sectionDisposables.Add(anySectionChanged.Connect());
                }));
    }

    /// <summary>
    /// Applies pending changes collected during the current update window as adapter batch operations.
    /// </summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <exception cref="InvalidOperationException">Thrown when called off the creating thread.</exception>
    private void ApplyPendingChanges(int sectionInfoId)
    {
        VerifyOnMainThread();
        Debug.Assert(_isCollectingChanges, "Currently there are no changes to collect");

        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Applying pending changes", sectionInfoId);

        try
        {
            _adapter.PerformUpdates(
                () =>
                {
                    if (_pendingChanges.Count == 0)
                    {
                        return;
                    }

                    if (IsDebugEnabled)
                    {
                        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] The pending changes (in order received) are:", sectionInfoId);

                        for (var i = 0; i < _pendingChanges.Count; i++)
                        {
                            var (section, pendingChange) = _pendingChanges[i];
                            this.Log().Debug(
                                CultureInfo.InvariantCulture,
                                "[#{0}] Section {1}: Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                                sectionInfoId,
                                section,
                                pendingChange.Action,
                                pendingChange.OldStartingIndex,
                                pendingChange.NewStartingIndex,
                                pendingChange.OldItems is null ? "null" : pendingChange.OldItems.Count.ToString(CultureInfo.InvariantCulture),
                                pendingChange.NewItems is null ? "null" : pendingChange.NewItems.Count.ToString(CultureInfo.InvariantCulture));
                        }
                    }

                    // Sort by section to process per section without GroupBy allocations.
                    _pendingChanges.Sort(static (a, b) => a.section.CompareTo(b.section));

                    var iChange = 0;
                    while (iChange < _pendingChanges.Count)
                    {
                        var section = _pendingChanges[iChange].section;

                        this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Processing updates for section {1}", sectionInfoId, section);

                        // Scan to determine the range [iChange, iEnd) for this section and whether any Reset is present.
                        var iEnd = iChange;
                        var hasReset = false;

                        while (iEnd < _pendingChanges.Count && _pendingChanges[iEnd].section == section)
                        {
                            if (_pendingChanges[iEnd].pendingChange.Action == NotifyCollectionChangedAction.Reset)
                            {
                                hasReset = true;
                            }

                            iEnd++;
                        }

                        if (hasReset)
                        {
                            this.Log().Debug(
                                CultureInfo.InvariantCulture,
                                "[#{0}] Section {1} included a reset notification, so reloading that section.",
                                sectionInfoId,
                                section);

                            _adapter.ReloadSections(new NSIndexSet((nuint)section));
                            iChange = iEnd;
                            continue;
                        }

                        // Materialize updates for this section.
                        // We keep using the existing normalization routine; updates list is per-section and bounded.
                        var updates = new List<Update>();

                        for (var j = iChange; j < iEnd; j++)
                        {
                            foreach (var update in GetUpdatesForEvent(_pendingChanges[j].pendingChange))
                            {
                                if (update is null)
                                {
                                    continue;
                                }

                                updates.Add(update);
                            }
                        }

                        var normalizedUpdates = IndexNormalizer.Normalize(updates);

                        if (IsDebugEnabled)
                        {
                            this.Log().Debug(
                                CultureInfo.InvariantCulture,
                                "[#{0}] Updates for section {1}: {2}",
                                sectionInfoId,
                                section,
                                string.Join(":", updates));

                            this.Log().Debug(
                                CultureInfo.InvariantCulture,
                                "[#{0}] Normalized updates for section {1}: {2}",
                                sectionInfoId,
                                section,
                                string.Join(":", normalizedUpdates));
                        }

                        for (var k = 0; k < normalizedUpdates.Count; k++)
                        {
                            var normalizedUpdate = normalizedUpdates[k];
                            switch (normalizedUpdate?.Type)
                            {
                                case UpdateType.Add:
                                    DoUpdate(_adapter.InsertItems, normalizedUpdate.Index, section);
                                    break;

                                case UpdateType.Delete:
                                    DoUpdate(_adapter.DeleteItems, normalizedUpdate.Index, section);
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }
                        }

                        iChange = iEnd;
                    }
                },
                () => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Pending changes applied", sectionInfoId));
        }
        finally
        {
            _pendingChanges.Clear();
        }
    }

    /// <summary>
    /// Applies an adapter update method for a single index within a section.
    /// </summary>
    /// <param name="method">The adapter method.</param>
    /// <param name="index">The row index.</param>
    /// <param name="section">The section index.</param>
    private void DoUpdate(Action<NSIndexPath[]> method, int index, int section)
    {
        // Single item -> avoid IEnumerable allocations and ToArray.
        var toChange = new[] { NSIndexPath.FromRowSection(index, section) };

        if (IsDebugEnabled)
        {
            this.Log().Debug(
                CultureInfo.InvariantCulture,
                "Calling {0}: [{1}]",
                method.Method.Name,
                toChange[0].Section + "-" + toChange[0].Row);
        }

        method(toChange);
    }

    /// <summary>
    /// Throws if the current thread is not the creating thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when called off the creating thread.</exception>
    private void VerifyOnMainThread()
    {
        if (Environment.CurrentManagedThreadId != _mainThreadId)
        {
            throw new InvalidOperationException(
                "An operation has occurred off the main thread that must be performed on it. Be sure to schedule changes to the underlying data correctly.");
        }
    }

    /// <summary>
    /// Snapshot of a collection change event that is resilient to external mutation of the original event args.
    /// </summary>
    /// <remarks>
    /// Rather than storing <see cref="NotifyCollectionChangedEventArgs"/> instances, we store instances of this type.
    /// Storing the event args directly does not always work because external code can mutate the instance before it
    /// can be applied.
    /// </remarks>
    private sealed class PendingChange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChange"/> class.
        /// </summary>
        /// <param name="ea">The original collection change event arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ea"/> is <see langword="null"/>.</exception>
        public PendingChange(NotifyCollectionChangedEventArgs ea)
        {
            ArgumentExceptionHelper.ThrowIfNull(ea);

            Action = ea.Action;
            OldItems = CopyItems(ea.OldItems);
            NewItems = CopyItems(ea.NewItems);
            OldStartingIndex = ea.OldStartingIndex;
            NewStartingIndex = ea.NewStartingIndex;
        }

        /// <summary>
        /// Gets the collection change action.
        /// </summary>
        public NotifyCollectionChangedAction Action { get; }

        /// <summary>
        /// Gets the copied old items.
        /// </summary>
        public IList? OldItems { get; }

        /// <summary>
        /// Gets the copied new items.
        /// </summary>
        public IList? NewItems { get; }

        /// <summary>
        /// Gets the old starting index.
        /// </summary>
        public int OldStartingIndex { get; }

        /// <summary>
        /// Gets the new starting index.
        /// </summary>
        public int NewStartingIndex { get; }

        /// <summary>
        /// Creates a shallow copy of the items in the specified list, returning a new list containing the same
        /// elements.
        /// </summary>
        /// <remarks>The copy is shallow; reference types within the list are not cloned. The returned
        /// list is always of type <see cref="List{Object}"/>.</remarks>
        /// <param name="source">The list whose items are to be copied. Can be null or empty.</param>
        /// <returns>A new list containing the elements of <paramref name="source"/>. Returns null if <paramref name="source"/>
        /// is null, or an empty list if <paramref name="source"/> is empty.</returns>
        private static IList? CopyItems(IList? source)
        {
            if (source is null || source.Count == 0)
            {
                return source is null ? null : Array.Empty<object>();
            }

            var list = new List<object>(source.Count);
            for (var i = 0; i < source.Count; i++)
            {
                list.Add(source[i]!);
            }

            return list;
        }
    }
}
