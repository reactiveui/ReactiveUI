// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>An AndroidX <see cref="AndroidX.ReactivePreferenceFragment{TViewModel}"/> with no preferences.</summary>
public class PlainPreferenceFragment : AndroidX.ReactivePreferenceFragment<TestViewModel>
{
    /// <inheritdoc/>
    public override void OnCreatePreferences(Bundle? savedInstanceState, string? rootKey)
    {
    }
}
