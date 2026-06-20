// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Foundation;
using ReactiveUI.Internal;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides a common reactive data source implementation for collection-based UI views backed by sectioned data.</summary>
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
[SuppressMessage("Minor Code Smell", "S2326:Unused type parameters should be removed", Justification = "TSource is part of the public API surface and provides call-site type-safety for consumers.")]
internal sealed class CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> : ReactiveObject, IDisposable
    where TSectionInfo : ISectionInformation<TUIViewCell>
{
    /// <summary>Adapter used to manipulate the UI view (reload, begin/end updates, insert/delete items, etc.).</summary>
    private readonly IUICollViewAdapter<TUIView, TUIViewCell> _adapter;

    /// <summary>Managed thread id captured at construction time; used to validate calls occur on the expected thread.</summary>
    private readonly int _mainThreadId;

    /// <summary>Root disposable for subscriptions created by this instance.</summary>
    private readonly MultipleDisposable _mainDisposables;

    /// <summary>Holds subscriptions associated with the current <see cref="SectionInfo"/> value. Replaced when SectionInfo changes.</summary>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Handled by the SwapDisposable")]
    private readonly SwapDisposable _sectionInfoDisposable;

    /// <summary>Pending collection changes captured while the UI is not reloading and before a scheduled batch update is applied.</summary>
    private readonly List<(int section, PendingChange pendingChange)> _pendingChanges;

    /// <summary>Indicates whether pending changes are currently being collected for later application.</summary>
    private bool _isCollectingChanges;

    /// <summary>Backing store for <see cref="SectionInfo"/>.</summary>
    private IReadOnlyList<TSectionInfo> _sectionInfo;

    /// <summary>Initializes a new instance of the <see cref="CommonReactiveSource{TSource, TUIView, TUIViewCell, TSectionInfo}"/> class.</summary>
    /// <param name="adapter">The adapter to use which we want to display information for.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="adapter"/> is <see langword="null"/>.</exception>
    public CommonReactiveSource(IUICollViewAdapter<TUIView, TUIViewCell> adapter)
    {
        ArgumentExceptionHelper.ThrowIfNull(adapter);

        _adapter = adapter;
        _mainThreadId = Environment.CurrentManagedThreadId;

        _mainDisposables = [];
        _sectionInfoDisposable = new SwapDisposable();
        _mainDisposables.Add(_sectionInfoDisposable);

        _pendingChanges = [];
        _sectionInfo = [];

        // Avoid ObservableForProperty/WhenAnyValue (expression trees); filter by property name instead.
        _mainDisposables.Add(
            new KeepSignal<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                    Changing!,
                    static e => e.PropertyName == nameof(SectionInfo))
                .Subscribe(new DelegateObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                    _ => SectionInfoChanging(),
                    ex => this.Log().Error(ex, "Error occurred whilst SectionInfo changing."))));

        _mainDisposables.Add(
            new KeepSignal<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                    Changed!,
                    static e => e.PropertyName == nameof(SectionInfo))
                .Subscribe(new DelegateObserver<IReactivePropertyChangedEventArgs<IReactiveObject>>(
                    _ => SectionInfoChanged(SectionInfo),
                    ex => this.Log().Error(ex, "Error occurred when SectionInfo changed."))));
    }

    /// <summary>Gets or sets the current section information.</summary>
    /// <remarks>
    /// Assigning this property replaces all subscriptions associated with the prior section information.
    /// </remarks>
    public IReadOnlyList<TSectionInfo> SectionInfo
    {
        get => _sectionInfo;
        set => this.RaiseAndSetIfChanged(ref _sectionInfo, value);
    }

    /// <summary>Gets a value indicating whether debug logging is enabled.</summary>
    private bool IsDebugEnabled => this.Log().Level <= LogLevel.Debug;

    /// <summary>Returns the number of sections.</summary>
    /// <returns>The number of sections.</returns>
    public int NumberOfSections()
    {
        VerifyOnMainThread();

        var count = _sectionInfo.Count;
        this.Log().Debug(CultureInfo.InvariantCulture, "Reporting number of sections = {0}", count);

        return count;
    }

    /// <summary>Returns the number of rows in a section.</summary>
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

    /// <summary>Returns the item at the specified index path.</summary>
    /// <param name="path">The index path.</param>
    /// <returns>The item at the index path, or <see langword="null"/>.</returns>
    public object? ItemAt(NSIndexPath path)
    {
        VerifyOnMainThread();

        var list = (IList)_sectionInfo[path.Section].Collection!;
        this.Log().Debug(CultureInfo.InvariantCulture, "Returning item at {0}-{1}", path.Section, path.Row);

        return list[path.Row];
    }

    /// <summary>Dequeues and configures a cell for the specified index path.</summary>
    /// <param name="indexPath">The index path.</param>
    /// <returns>The configured view cell.</returns>
    public TUIViewCell GetCell(NSIndexPath indexPath)
    {
        VerifyOnMainThread();

        this.Log().Debug(CultureInfo.InvariantCulture, "Getting cell for index path {0}-{1}", indexPath.Section, indexPath.Row);

        var section = _sectionInfo[indexPath.Section];
        var viewModel = ((IList)section.Collection!)[indexPath.Row];

        var key = section?.CellKeySelector?.Invoke(viewModel) ?? NSString.Empty;
        var cell = _adapter.DequeueReusableCell(key, indexPath);

        if (cell is IViewFor view)
        {
            this.Log().Debug(CultureInfo.InvariantCulture, "Setting VM for index path {0}-{1}", indexPath.Section, indexPath.Row);
            view.ViewModel = viewModel;
        }

        var initializeCellAction = section?.InitializeCellAction ?? NoOpInitializeCell;
        initializeCellAction(cell);

        return cell;
    }

    /// <summary>Disposes subscriptions and managed resources associated with this instance.</summary>
    public void Dispose()
    {
        _mainDisposables.Dispose();
    }

    /// <summary>No-op initializer used when a section does not provide an initialization callback.</summary>
    /// <param name="cell">The cell to initialize.</param>
    private static void NoOpInitializeCell(TUIViewCell cell)
    {
        // Intentionally empty: used as a no-op default when no section-level initialization callback is provided.
        // The cell parameter is required to match the Action<TUIViewCell> delegate signature.
        _ = cell;
    }

    /// <summary>Builds the list of updates for a pending collection change event.</summary>
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
                GetRemoveUpdates(pendingChange),

            // Use OldStartingIndex for each "Update.Index" because the batch update processes and removes items sequentially
            // opposed to as one Range operation.
            NotifyCollectionChangedAction.Move =>
                GetMoveUpdates(pendingChange),

            NotifyCollectionChangedAction.Replace =>
                GetReplaceUpdates(pendingChange),

            _ => throw new NotSupportedException("Don't know how to deal with " + pendingChange.Action),
        };

    /// <summary>Builds the delete updates for a <see cref="NotifyCollectionChangedAction.Remove"/> change.</summary>
    /// <param name="pendingChange">The pending change.</param>
    /// <returns>An enumerable of delete updates.</returns>
    private static IEnumerable<Update> GetRemoveUpdates(PendingChange pendingChange) =>
        Enumerable
            .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
            .Select(_ => Update.CreateDelete(pendingChange.OldStartingIndex));

    /// <summary>Builds the delete-then-add updates for a <see cref="NotifyCollectionChangedAction.Move"/> change.</summary>
    /// <param name="pendingChange">The pending change.</param>
    /// <returns>An enumerable of delete and add updates.</returns>
    private static IEnumerable<Update> GetMoveUpdates(PendingChange pendingChange) =>
        Enumerable
            .Range(pendingChange.OldStartingIndex, pendingChange.OldItems is null ? 1 : pendingChange.OldItems.Count)
            .Select(Update.CreateDelete)
            .Concat(
                Enumerable
                    .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
                    .Select(Update.CreateAdd));

    /// <summary>Builds the delete-then-add updates for a <see cref="NotifyCollectionChangedAction.Replace"/> change.</summary>
    /// <param name="pendingChange">The pending change.</param>
    /// <returns>An enumerable of paired delete and add updates.</returns>
    private static IEnumerable<Update> GetReplaceUpdates(PendingChange pendingChange) =>
        Enumerable
            .Range(pendingChange.NewStartingIndex, pendingChange.NewItems is null ? 1 : pendingChange.NewItems.Count)
            .SelectMany(static x => new[] { Update.CreateDelete(x), Update.CreateAdd(x) });

    /// <summary>Called before <see cref="SectionInfo"/> changes. Disposes subscriptions for the current SectionInfo.</summary>
    private void SectionInfoChanging()
    {
        VerifyOnMainThread();

        this.Log().Debug(CultureInfo.InvariantCulture, "SectionInfo about to change, disposing of any subscriptions for previous SectionInfo.");
        _sectionInfoDisposable.Disposable = null;
    }

    /// <summary>Called when <see cref="SectionInfo"/> changes. Subscribes to section structure and item changes.</summary>
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

        var sectionChanged = new StartWithObservable<RxVoid>(
            notifyCollectionChanged is null
                ? Signal.Silent<RxVoid>()
                : new MapSignal<CollectionChanged, RxVoid>(
                    notifyCollectionChanged.ObserveCollectionChanges(),
                    static _ => RxVoid.Default),
            RxVoid.Default);

        var disposables = new MultipleDisposable
        {
            Scope.Create(() => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Disposed of section info", sectionInfoId))
        };

        _sectionInfoDisposable.Disposable = disposables;
        SubscribeToSectionInfoChanges(sectionInfoId, sectionInfo, sectionChanged, disposables);
    }

    /// <summary>Subscribes to changes in the section collection and to changes within each section's item collection.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="sectionInfo">The current section info.</param>
    /// <param name="sectionChanged">An observable indicating that the section set changed.</param>
    /// <param name="disposables">A disposable container for subscriptions.</param>
    private void SubscribeToSectionInfoChanges(int sectionInfoId, IReadOnlyList<TSectionInfo> sectionInfo, StartWithObservable<RxVoid> sectionChanged, MultipleDisposable disposables)
    {
        // holds a single disposable representing the monitoring of sectionInfo.
        // once disposed, any changes to sectionInfo will no longer trigger any of the logic below
        var sectionInfoDisposable = new SwapDisposable();
        disposables.Add(sectionInfoDisposable);

        disposables.Add(
            sectionChanged.Subscribe(new DelegateObserver<RxVoid>(
                _ =>
                {
                    VerifyOnMainThread();

                    this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Calling ReloadData()", sectionInfoId);
                    _adapter.ReloadData();

                    var sectionDisposables = new MultipleDisposable();
                    sectionInfoDisposable.Disposable = sectionDisposables;

                    var applyPendingChangesDisposable = new SwapDisposable();
                    sectionDisposables.Add(applyPendingChangesDisposable);

                    SetUpSectionChangeSubscriptions(sectionInfoId, sectionInfo, sectionDisposables, applyPendingChangesDisposable);
                })));
    }

    /// <summary>
    /// Attaches the per-section change subscriptions, routing each collection change according to the adapter's
    /// current reload state. Replaces the previous <c>Publish</c>/<c>Join</c>/<c>Connect</c> multicast pipeline with
    /// a single purpose-built sink (<see cref="ReloadAwareSectionSink"/>).
    /// </summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="sectionInfo">The current section info.</param>
    /// <param name="sectionDisposables">Container for all subscriptions created here.</param>
    /// <param name="applyPendingChangesDisposable">Serial disposable that holds the scheduled apply-changes action.</param>
    private void SetUpSectionChangeSubscriptions(
        int sectionInfoId,
        IReadOnlyList<TSectionInfo> sectionInfo,
        MultipleDisposable sectionDisposables,
        SwapDisposable applyPendingChangesDisposable)
    {
        var sink = new ReloadAwareSectionSink(this, sectionInfoId, applyPendingChangesDisposable);
        sectionDisposables.Add(sink);
        sink.Run(sectionInfo);
    }

    /// <summary>Handles a single item-change event received from a section while no reload is in progress.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="applyPendingChangesDisposable">Serial disposable used to schedule the deferred apply-changes action.</param>
    /// <param name="change">The collection change event wrapper.</param>
    /// <param name="section">The zero-based index of the section that changed.</param>
    private void OnSectionItemChanged(
        int sectionInfoId,
        SwapDisposable applyPendingChangesDisposable,
        CollectionChanged change,
        int section)
    {
        VerifyOnMainThread();

        if (IsDebugEnabled)
        {
            var eventArgs = change.EventArgs;
            this.Log().Debug(
                CultureInfo.InvariantCulture,
                "[#{0}] Change detected in section {1} : Action = {2}, OldStartingIndex = {3}, NewStartingIndex = {4}, OldItems.Count = {5}, NewItems.Count = {6}",
                sectionInfoId,
                section,
                eventArgs.Action,
                eventArgs.OldStartingIndex,
                eventArgs.NewStartingIndex,
                eventArgs.OldItems is null ? "null" : eventArgs.OldItems.Count.ToString(CultureInfo.InvariantCulture),
                eventArgs.NewItems is null ? "null" : eventArgs.NewItems.Count.ToString(CultureInfo.InvariantCulture));
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

        _pendingChanges.Add((section, new PendingChange(change.EventArgs)));
    }

    /// <summary>Applies pending changes collected during the current update window as adapter batch operations.</summary>
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
                () => ApplyPendingChangesCore(sectionInfoId),
                () => this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Pending changes applied", sectionInfoId));
        }
        finally
        {
            _pendingChanges.Clear();
        }
    }

    /// <summary>Core logic executed inside the adapter's <c>PerformUpdates</c> action: logs, sorts and dispatches each pending change.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    private void ApplyPendingChangesCore(int sectionInfoId)
    {
        if (_pendingChanges.Count == 0)
        {
            return;
        }

        LogPendingChanges(sectionInfoId);

        // Sort by section to process per section without GroupBy allocations.
        _pendingChanges.Sort(static (a, b) => a.section.CompareTo(b.section));

        var changeIndex = 0;
        while (changeIndex < _pendingChanges.Count)
        {
            var section = _pendingChanges[changeIndex].section;
            this.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] Processing updates for section {1}", sectionInfoId, section);
            changeIndex = ProcessSectionUpdates(sectionInfoId, section, changeIndex);
        }
    }

    /// <summary>Logs all pending changes when debug logging is enabled.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    private void LogPendingChanges(int sectionInfoId)
    {
        if (!IsDebugEnabled)
        {
            return;
        }

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

    /// <summary>Processes all pending changes for a single section starting at <paramref name="changeIndex"/>.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="section">The section index being processed.</param>
    /// <param name="changeIndex">The index into <see cref="_pendingChanges"/> at which this section starts.</param>
    /// <returns>The next value of <c>changeIndex</c> (i.e. the first index belonging to the next section).</returns>
    private int ProcessSectionUpdates(int sectionInfoId, int section, int changeIndex)
    {
        // Scan to determine the range [changeIndex, endIndex) for this section and whether any Reset is present.
        var endIndex = changeIndex;
        var hasReset = false;

        while (endIndex < _pendingChanges.Count && _pendingChanges[endIndex].section == section)
        {
            if (_pendingChanges[endIndex].pendingChange.Action == NotifyCollectionChangedAction.Reset)
            {
                hasReset = true;
            }

            endIndex++;
        }

        if (hasReset)
        {
            this.Log().Debug(
                CultureInfo.InvariantCulture,
                "[#{0}] Section {1} included a reset notification, so reloading that section.",
                sectionInfoId,
                section);

            _adapter.ReloadSections(new NSIndexSet((nuint)section));
            return endIndex;
        }

        ApplySectionItemUpdates(sectionInfoId, section, changeIndex, endIndex);
        return endIndex;
    }

    /// <summary>Materializes, normalizes, and applies item-level insert/delete operations for a single section.</summary>
    /// <param name="sectionInfoId">A correlation id for logging.</param>
    /// <param name="section">The section index.</param>
    /// <param name="changeIndex">Inclusive start index into <see cref="_pendingChanges"/> for this section.</param>
    /// <param name="endIndex">Exclusive end index into <see cref="_pendingChanges"/> for this section.</param>
    private void ApplySectionItemUpdates(int sectionInfoId, int section, int changeIndex, int endIndex)
    {
        // Materialize updates for this section.
        // We keep using the existing normalization routine; updates list is per-section and bounded.
        var updates = new List<Update>();

        for (var j = changeIndex; j < endIndex; j++)
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
            if (normalizedUpdate is null)
            {
                continue;
            }

            switch (normalizedUpdate.Type)
            {
                case UpdateType.Add:
                {
                    DoUpdate(_adapter.InsertItems, normalizedUpdate.Index, section);
                    break;
                }

                case UpdateType.Delete:
                {
                    DoUpdate(_adapter.DeleteItems, normalizedUpdate.Index, section);
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>Applies an adapter update method for a single index within a section.</summary>
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

    /// <summary>Throws if the current thread is not the creating thread.</summary>
    /// <exception cref="InvalidOperationException">Thrown when called off the creating thread.</exception>
    private void VerifyOnMainThread()
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            return;
        }

        throw new InvalidOperationException(
            "An operation has occurred off the main thread that must be performed on it. Be sure to schedule changes to the underlying data correctly.");
    }

    /// <summary>
    /// Routes per-section collection changes according to the adapter's current reload state. While a reload is in
    /// progress every section change forces another reload; otherwise the change is collected for batch application.
    /// Replaces the previous <c>Publish</c>/<c>Join</c>/<c>Connect</c> multicast pipeline with a single sink that
    /// tracks the reload state and dispatches changes directly.
    /// </summary>
    private sealed class ReloadAwareSectionSink : IDisposable
    {
        /// <summary>The owning source whose adapter and state the sink drives.</summary>
        private readonly CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> _parent;

        /// <summary>A correlation id for logging.</summary>
        private readonly int _sectionInfoId;

        /// <summary>Serial disposable that holds the scheduled apply-changes action.</summary>
        private readonly SwapDisposable _applyPendingChangesDisposable;

        /// <summary>All subscriptions created by this sink.</summary>
        private readonly MultipleDisposable _subscriptions = [];

        /// <summary>The latest observed reload state.</summary>
        private bool _isReloading;

        /// <summary>Whether a reload-state value has been observed yet.</summary>
        private bool _hasReloadingValue;

        /// <summary>Initializes a new instance of the <see cref="ReloadAwareSectionSink"/> class.</summary>
        /// <param name="parent">The owning source.</param>
        /// <param name="sectionInfoId">A correlation id for logging.</param>
        /// <param name="applyPendingChangesDisposable">Serial disposable that holds the scheduled apply-changes action.</param>
        public ReloadAwareSectionSink(
            CommonReactiveSource<TSource, TUIView, TUIViewCell, TSectionInfo> parent,
            int sectionInfoId,
            SwapDisposable applyPendingChangesDisposable)
        {
            _parent = parent;
            _sectionInfoId = sectionInfoId;
            _applyPendingChangesDisposable = applyPendingChangesDisposable;
        }

        /// <summary>Subscribes to the adapter reload state and to each section's collection changes.</summary>
        /// <param name="sectionInfo">The current section info.</param>
        public void Run(IReadOnlyList<TSectionInfo> sectionInfo)
        {
            // IsReloadingData is a BehaviorSubject, so the current value arrives synchronously on subscription and is
            // in place before any section-change notification is dispatched below.
            _subscriptions.Add(_parent._adapter.IsReloadingData.Subscribe(new DelegateObserver<bool>(OnReloadingChanged)));

            for (var index = 0; index < sectionInfo.Count; index++)
            {
                var section = index;
                _subscriptions.Add(
                    sectionInfo[section].Collection!.ObserveCollectionChanges().Subscribe(
                        new DelegateObserver<CollectionChanged>(
                            change => OnSectionChanged(change, section),
                            ex => _parent.Log().Error(CultureInfo.InvariantCulture, "[#{0}] Error while watching section collection: {1}", _sectionInfoId, ex))));
            }
        }

        /// <inheritdoc/>
        public void Dispose() => _subscriptions.Dispose();

        /// <summary>Records the latest reload state, logging only when it changes.</summary>
        /// <param name="value">The new reload state.</param>
        private void OnReloadingChanged(bool value)
        {
            if (_hasReloadingValue && _isReloading == value)
            {
                return;
            }

            _isReloading = value;
            _hasReloadingValue = true;
            _parent.Log().Debug(CultureInfo.InvariantCulture, "[#{0}] IsReloadingData = {1}", _sectionInfoId, value);
        }

        /// <summary>Dispatches a single section change based on the current reload state.</summary>
        /// <param name="change">The collection change.</param>
        /// <param name="section">The zero-based section index.</param>
        private void OnSectionChanged(CollectionChanged change, int section)
        {
            _parent.VerifyOnMainThread();

            if (_isReloading)
            {
                // A section changed whilst a reload is in progress - force another reload so the change is not lost.
                _parent.Log().Debug(
                    CultureInfo.InvariantCulture,
                    "[#{0}] A section changed whilst a reload is in progress - forcing another reload.",
                    _sectionInfoId);

                _parent._adapter.ReloadData();
                _parent._pendingChanges.Clear();
                _parent._isCollectingChanges = false;
                return;
            }

            _parent.OnSectionItemChanged(_sectionInfoId, _applyPendingChangesDisposable, change, section);
        }
    }

    /// <summary>Snapshot of a collection change event that is resilient to external mutation of the original event args.</summary>
    /// <remarks>
    /// Rather than storing <see cref="NotifyCollectionChangedEventArgs"/> instances, we store instances of this type.
    /// Storing the event args directly does not always work because external code can mutate the instance before it
    /// can be applied.
    /// </remarks>
    private sealed class PendingChange
    {
        /// <summary>Initializes a new instance of the <see cref="PendingChange"/> class.</summary>
        /// <param name="eventArgs">The original collection change event arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventArgs"/> is <see langword="null"/>.</exception>
        public PendingChange(NotifyCollectionChangedEventArgs eventArgs)
        {
            ArgumentExceptionHelper.ThrowIfNull(eventArgs);

            Action = eventArgs.Action;
            OldItems = CopyItems(eventArgs.OldItems);
            NewItems = CopyItems(eventArgs.NewItems);
            OldStartingIndex = eventArgs.OldStartingIndex;
            NewStartingIndex = eventArgs.NewStartingIndex;
        }

        /// <summary>Gets the collection change action.</summary>
        public NotifyCollectionChangedAction Action { get; }

        /// <summary>Gets the copied old items.</summary>
        public IList? OldItems { get; }

        /// <summary>Gets the copied new items.</summary>
        public IList? NewItems { get; }

        /// <summary>Gets the old starting index.</summary>
        public int OldStartingIndex { get; }

        /// <summary>Gets the new starting index.</summary>
        public int NewStartingIndex { get; }

        /// <summary>Creates a shallow copy of the items in the specified list, returning a new list containing the same elements.</summary>
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
