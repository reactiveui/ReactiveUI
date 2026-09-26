// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows computing a property with <c>ObservableAsPropertyHelper</c> instead of setting it explicitly from a subscription.</summary>
public static class PreferOaphOverPropertiesExamples
{
    /// <summary>A plain settable property can be written to from anywhere, including by mistake, which leaves the view model in an inconsistent state.</summary>
    public static void AvoidASettablePropertyAnyoneCanOverwrite()
    {
        using AvoidEnrollmentViewModel viewModel = new();
        viewModel.RosterFetched = true;
        viewModel.RegistrarFree = true;

        Console.WriteLine(viewModel.CanEnroll);

        // Some other code, far away in the codebase, writes to the same property directly.
        viewModel.CanEnroll = false;

        Console.WriteLine(viewModel.CanEnroll);

        // Output:
        // True
        // False
    }

    /// <summary>An <c>ObservableAsPropertyHelper</c>-backed property has no setter, so nothing else in the codebase can write the wrong value into it.</summary>
    public static void PreferAnObservableAsPropertyHelper()
    {
        using PreferEnrollmentViewModel viewModel = new();
        viewModel.RosterFetched = true;
        viewModel.RegistrarFree = true;

        Console.WriteLine(viewModel.CanEnroll);

        // CanEnroll only changes when RosterFetched or RegistrarFree does; there is no other way to change it.
        viewModel.RegistrarFree = false;

        Console.WriteLine(viewModel.CanEnroll);

        // Output:
        // True
        // False
    }
}
