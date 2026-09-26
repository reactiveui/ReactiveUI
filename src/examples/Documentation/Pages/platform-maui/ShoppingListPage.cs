// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// The shopping plug-in's page for <see cref="ShoppingListViewModel"/>, pushed by a routed host. Like
/// <see cref="ShoppingListView"/>, it is registered with the service locator only.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("ShoppingListPage")]
public sealed class ShoppingListPage : ReactiveContentPage<ShoppingListViewModel>;
