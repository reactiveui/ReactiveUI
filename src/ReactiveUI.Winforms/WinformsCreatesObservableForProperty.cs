// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Winforms;

/// <summary>
/// WinForm view objects are not Generally Observable™, so hard-code some
/// particularly useful types.
/// </summary>
/// <seealso cref="ICreatesObservableForProperty" />
#if NET6_0_OR_GREATER
[RequiresUnreferencedCode("WinformsCreatesObservableForProperty uses methods that may require unreferenced code")]
#endif
public class WinformsCreatesObservableForProperty : ICreatesObservableForProperty
{
    private static readonly MemoizingMRUCache<(Type type, string name), EventInfo?> EventInfoCache = new(
     static (pair, _) => pair.type.GetEvent(pair.name + "Changed", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public),
     RxApp.SmallCacheLimit);

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        var supportsTypeBinding = typeof(Component).IsAssignableFrom(type);
        if (!supportsTypeBinding)
        {
            return 0;
        }

        var ei = EventInfoCache.Get((type, propertyName));
        return !beforeChanged && ei is not null ? 8 : 0;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
#endif
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(sender);
#else
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }
#endif

        var ei = EventInfoCache.Get((sender.GetType(), propertyName)) ?? throw new InvalidOperationException("Could not find a valid event for expression.");
        return Observable.Create<IObservedChange<object, object?>>(subj =>
        {
            var completed = false;
            var handler = new EventHandler((o, e) =>
            {
                if (completed)
                {
                    return;
                }

                try
                {
                    subj.OnNext(new ObservedChange<object, object?>(sender, expression, default));
                }
                catch (Exception ex)
                {
                    subj.OnError(ex);
                    completed = true;
                }
            });

            ei.AddEventHandler(sender, handler);
            return Disposable.Create(() => ei.RemoveEventHandler(sender, handler));
        });
    }
}
