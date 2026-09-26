// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>The lesson-notification settings screen, backed by <see cref="SettingsViewModel"/> and shared preferences.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class SettingsPreferenceFragment : AndroidX.ReactivePreferenceFragment<SettingsViewModel>
{
    /// <summary>The subscription to shared-preference changes, torn down when the fragment is destroyed.</summary>
    private IDisposable? _preferenceChangedSubscription;

    /// <inheritdoc/>
    public override void OnCreatePreferences(Bundle? savedInstanceState, string? rootKey)
    {
        ViewModel = new SettingsViewModel();
        SetPreferencesFromResource(Resource.Xml.settings_preferences, rootKey);

        ISharedPreferences? sharedPreferences = PreferenceManager?.SharedPreferences;
        if (sharedPreferences is null)
        {
            return;
        }

        _preferenceChangedSubscription = sharedPreferences.PreferenceChanged()
            .Subscribe(static key => TimetableLog.Info($"Preference changed: {key}."));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _preferenceChangedSubscription?.Dispose();
        }

        base.Dispose(disposing);
    }
}
