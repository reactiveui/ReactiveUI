// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// The view <see cref="ViewModelViewHost"/> shows for <see cref="RecipeListViewModel"/> under the "Wide" contract, for a
/// tablet in landscape. <see cref="ViewContractAttribute"/> puts it in the generated view lookup under that contract.
/// </summary>
[ViewContract("Wide")]
[System.Diagnostics.DebuggerDisplay("RecipeListWideContentView")]
public sealed class RecipeListWideContentView : ReactiveContentView<RecipeListViewModel>;
