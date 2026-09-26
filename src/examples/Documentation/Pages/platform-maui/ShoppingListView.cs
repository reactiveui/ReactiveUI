// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// The shopping plug-in's view for <see cref="ShoppingListViewModel"/>. The plug-in registers it with the service
/// locator only. <see cref="ExcludeFromViewRegistrationAttribute"/> keeps it out of the generated view lookup, the way a
/// view from a library built without the source generator is.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("ShoppingListView")]
public sealed class ShoppingListView : ReactiveContentView<ShoppingListViewModel>;
