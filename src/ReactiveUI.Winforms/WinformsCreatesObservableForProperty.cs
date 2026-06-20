// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI.Internal;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>WinForm view objects are not Generally Observable™, so hard-code some particularly useful types.</summary>
/// <seealso cref="ICreatesObservableForProperty" />
public class WinformsCreatesObservableForProperty : ICreatesObservableForProperty
{
    /// <summary>The affinity returned when a matching change event is found for a property.</summary>
    private const int EventBindingAffinity = 8;

    /// <summary>Caches the reflected change event for each property to avoid repeated reflection.</summary>
    private static readonly MemoizingMRUCache<(Type type, string name), EventInfo?> EventInfoCache = new(
        static (pair, _) => pair.type.GetEvent(
            pair.name + "Changed",
            BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public),
        RxCacheSize.SmallCacheLimit);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        if (type is null)
        {
            return 0;
        }

        var supportsTypeBinding = typeof(Component).IsAssignableFrom(type);
        if (!supportsTypeBinding)
        {
            return 0;
        }

        var ei = EventInfoCache.Get((type, propertyName));
        return !beforeChanged && ei is not null ? EventBindingAffinity : 0;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);

        var ei = EventInfoCache.Get((sender.GetType(), propertyName)) ??
                 throw new InvalidOperationException("Could not find a valid event for expression.");

        return new FromEventObservable<IObservedChange<object, object?>>(onNext =>
        {
            var handler = new EventHandler((_, _) =>
                onNext(new ObservedChange<object, object?>(sender, expression, null)));

            ei.AddEventHandler(sender, handler);

            // Unwire synchronously, mirroring the synchronous AddEventHandler above: disposing the subscription must
            // detach the handler immediately so no later change raises it. Marshalling the removal onto a scheduler
            // left the handler live until the next pump, so a change between Dispose and that pump still emitted.
            return new ActionDisposable(() => ei.RemoveEventHandler(sender, handler));
        });
    }
}
