// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods for shared preferences.</summary>
public static class SharedPreferencesExtensions
{
    /// <summary>Extends an <see cref="ISharedPreferences"/> instance with reactive change observation.</summary>
    /// <param name="sharedPreferences">The shared preferences whose key changes are observed.</param>
    extension(ISharedPreferences sharedPreferences)
    {
        /// <summary>A observable sequence of keys for changed shared preferences.</summary>
        /// <returns>The observable sequence of keys for changed shared preferences.</returns>
        public IObservable<string?> PreferenceChanged() =>
            new PreferenceChangedObservable(sharedPreferences);
    }

    /// <summary>
    /// Registers a change listener on subscribe and surfaces each changed key — replacing <c>Observable.Create</c>.
    /// The listener is unregistered when the subscription is disposed.
    /// </summary>
    /// <param name="sharedPreferences">The shared preferences to observe.</param>
    private sealed class PreferenceChangedObservable(ISharedPreferences sharedPreferences) : IObservable<string?>
    {
        /// <summary>Subscribes the given observer to the shared preference change notifications.</summary>
        /// <param name="observer">The observer to receive the changed keys.</param>
        /// <returns>A disposable that unregisters the change listener when disposed.</returns>
        public IDisposable Subscribe(IObserver<string?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            OnSharedPreferenceChangeListener listener = new(observer);
            sharedPreferences.RegisterOnSharedPreferenceChangeListener(listener);
            return new ActionDisposable(() => sharedPreferences.UnregisterOnSharedPreferenceChangeListener(listener));
        }
    }

    /// <summary>Private implementation of ISharedPreferencesOnSharedPreferenceChangeListener.</summary>
    /// <param name="observer">The observer that receives each changed preference key.</param>
    private sealed class OnSharedPreferenceChangeListener(IObserver<string?> observer)
        : Java.Lang.Object,
            ISharedPreferencesOnSharedPreferenceChangeListener
    {
        /// <inheritdoc/>
        void ISharedPreferencesOnSharedPreferenceChangeListener.OnSharedPreferenceChanged(
            ISharedPreferences? sharedPreferences,
            string? key) => observer.OnNext(key);
    }
}
