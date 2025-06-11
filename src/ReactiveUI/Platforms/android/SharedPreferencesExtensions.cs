// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

namespace ReactiveUI;

/// <summary>
/// Extension methods for shared preferences.
/// </summary>
public static class SharedPreferencesExtensions
{
    /// <summary>
    /// A observable sequence of keys for changed shared preferences.
    /// </summary>
    /// <returns>The observable sequence of keys for changed shared preferences.</returns>
    /// <param name="sharedPreferences">The shared preferences to get the changes from.</param>
    public static IObservable<string?> PreferenceChanged(this ISharedPreferences sharedPreferences) => // TODO: Create Test
        Observable.Create<string?>(observer =>
        {
            var listener = new OnSharedPreferenceChangeListener(observer);
            sharedPreferences.RegisterOnSharedPreferenceChangeListener(listener);
            return Disposable.Create(() => sharedPreferences.UnregisterOnSharedPreferenceChangeListener(listener));
        });

    /// <summary>
    /// Private implementation of ISharedPreferencesOnSharedPreferenceChangeListener.
    /// </summary>
    private class OnSharedPreferenceChangeListener(IObserver<string?> observer)
                : Java.Lang.Object,
            ISharedPreferencesOnSharedPreferenceChangeListener
    {
        void ISharedPreferencesOnSharedPreferenceChangeListener.OnSharedPreferenceChanged(ISharedPreferences? sharedPreferences, string? key) => observer.OnNext(key);
    }
}
