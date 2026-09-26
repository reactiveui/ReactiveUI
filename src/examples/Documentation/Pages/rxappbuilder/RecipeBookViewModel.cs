// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The view model behind the recipe book's home screen.</summary>
[System.Diagnostics.DebuggerDisplay("RecipeBookViewModel Title = {Title}")]
public sealed class RecipeBookViewModel : ReactiveObject
{
    /// <summary>Gets or sets the title shown at the top of the screen.</summary>
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Weeknight Dinners";
}
