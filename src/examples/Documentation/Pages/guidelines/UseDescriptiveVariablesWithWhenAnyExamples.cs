// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows naming each side of a multi-property <c>WhenAny</c> so a boolean expression stays readable.</summary>
public static class UseDescriptiveVariablesWithWhenAnyExamples
{
    /// <summary>Naming the selector's parameters after what they mean makes the expression read like the rule it checks.</summary>
    public static void PreferDescriptiveParameterNames()
    {
        Student student = new("Ada", "Robotics Club");
        List<string> log = [];
        using IDisposable subscription = student.WhenAny(
                x => x.Enrollment.IsEnabled,
                x => x.Enrollment.IsLoading,
                static (isEnabled, isLoading) => isEnabled.Value && isLoading.Value)
            .Subscribe(canJoin => log.Add(canJoin ? "Can join now" : "Cannot join yet"));

        student.Enrollment.IsLoading = true;

        Console.WriteLine(string.Join(", ", log));

        // Output:
        // Cannot join yet, Can join now
    }

    /// <summary>The same rule with <c>x</c> and <c>y</c> compiles and behaves exactly the same, but a reader has to open the selector to know which is which.</summary>
    public static void AvoidUnnamedParameters()
    {
        Student student = new("Ada", "Robotics Club");
        List<string> log = [];
        using IDisposable subscription = student.WhenAny(
                x => x.Enrollment.IsEnabled,
                x => x.Enrollment.IsLoading,
                static (x, y) => x.Value && y.Value)
            .Subscribe(canJoin => log.Add(canJoin ? "Can join now" : "Cannot join yet"));

        student.Enrollment.IsLoading = true;

        Console.WriteLine(string.Join(", ", log));

        // Output:
        // Cannot join yet, Can join now
    }
}
