// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>Shows that a hand-rolled <see cref="IReactiveObject"/> implementation, such as <see cref="CourseResultEntity"/>, behaves the same as a <see cref="ReactiveObject"/>.</summary>
public static class ManualReactiveObjectExamples
{
    /// <summary>The classic events, the <c>Changed</c> observable, and suppression all work the same way as on a <see cref="ReactiveObject"/>.</summary>
    public static void RaiseAndSuppressLikeAReactiveObject()
    {
        CourseResultEntity result = new() { Course = "Databases", Grade = 70 };
        int classicChanging = 0;
        int classicChanged = 0;
        List<int> observedGrades = [];
        result.PropertyChanging += (_, _) => classicChanging++;
        result.PropertyChanged += (_, _) => classicChanged++;
        using IDisposable subscription = result.Changed
            .Where(static args => args.PropertyName == nameof(CourseResultEntity.Grade))
            .Subscribe(_ => observedGrades.Add(result.Grade));

        result.Grade = 74;

        using (result.SuppressChangeNotifications())
        {
            Console.WriteLine(result.AreChangeNotificationsEnabled());
            result.Grade = 80;
        }

        Console.WriteLine(result.AreChangeNotificationsEnabled());
        Console.WriteLine(classicChanging);
        Console.WriteLine(classicChanged);
        Console.WriteLine(string.Join(", ", observedGrades));

        // Output:
        // False
        // True
        // 1
        // 1
        // 74
    }
}
