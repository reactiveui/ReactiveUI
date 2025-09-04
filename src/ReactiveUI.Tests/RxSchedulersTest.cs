// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the RxSchedulers class to ensure it works without RequiresUnreferencedCode attributes.
/// </summary>
public class RxSchedulersTest
{
    /// <summary>
    /// Tests that schedulers can be accessed without attributes.
    /// </summary>
    [Fact]
    public void SchedulersCanBeAccessedWithoutAttributes()
    {
        // This test method itself should not require RequiresUnreferencedCode
        // because it uses RxSchedulers instead of RxApp
        
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;
        
        Assert.NotNull(mainScheduler);
        Assert.NotNull(taskpoolScheduler);
    }

    /// <summary>
    /// Tests that schedulers can be set and retrieved.
    /// </summary>
    [Fact]
    public void SchedulersCanBeSetAndRetrieved()
    {
        var testScheduler = new TestScheduler();
        
        // Set schedulers
        RxSchedulers.MainThreadScheduler = testScheduler;
        RxSchedulers.TaskpoolScheduler = testScheduler;
        
        // Verify they were set
        Assert.Equal(testScheduler, RxSchedulers.MainThreadScheduler);
        Assert.Equal(testScheduler, RxSchedulers.TaskpoolScheduler);
        
        // Reset to defaults
        RxSchedulers.MainThreadScheduler = DefaultScheduler.Instance;
        RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
    }

    /// <summary>
    /// Tests that RxSchedulers provides basic scheduler functionality.
    /// </summary>
    [Fact]
    public void SchedulersProvideBasicFunctionality()
    {
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;
        
        // Verify they implement IScheduler
        Assert.IsAssignableFrom<IScheduler>(mainScheduler);
        Assert.IsAssignableFrom<IScheduler>(taskpoolScheduler);
        
        // Verify they have Now property
        Assert.True(mainScheduler.Now > DateTimeOffset.MinValue);
        Assert.True(taskpoolScheduler.Now > DateTimeOffset.MinValue);
    }
}