// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>Tests for <see cref="ReactiveNotificationHelpers"/>.</summary>
public class ReactiveNotificationHelpersTests
{
    /// <summary>The number of handlers registered in combination tests.</summary>
    private const int ExpectedHandlerCalls = 2;

    /// <summary>Verifies that property-changing handlers are initialized and retained.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddPropertyChanging_NewHandlers_InitializesAndCombinesHandlers()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        PropertyChangingEventHandler? handlers = null;
        var calls = 0;

        ReactiveNotificationHelpers.AddPropertyChanging(source, ref subscribed, ref handlers, (_, _) => calls++);
        ReactiveNotificationHelpers.AddPropertyChanging(source, ref subscribed, ref handlers, (_, _) => calls++);
        handlers?.Invoke(source, new PropertyChangingEventArgs(nameof(TestReactiveObject.Value)));

        using (Assert.Multiple())
        {
            await Assert.That(subscribed).IsTrue();
            await Assert.That(calls).IsEqualTo(ExpectedHandlerCalls);
        }
    }

    /// <summary>Verifies that property-changed handlers are initialized and retained.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddPropertyChanged_NewHandlers_InitializesAndCombinesHandlers()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        PropertyChangedEventHandler? handlers = null;
        var calls = 0;

        ReactiveNotificationHelpers.AddPropertyChanged(source, ref subscribed, ref handlers, (_, _) => calls++);
        ReactiveNotificationHelpers.AddPropertyChanged(source, ref subscribed, ref handlers, (_, _) => calls++);
        handlers?.Invoke(source, new PropertyChangedEventArgs(nameof(TestReactiveObject.Value)));

        using (Assert.Multiple())
        {
            await Assert.That(subscribed).IsTrue();
            await Assert.That(calls).IsEqualTo(ExpectedHandlerCalls);
        }
    }

    /// <summary>Verifies that the property-changing observable is cached after its first creation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetChanging_RepeatedCalls_ReturnsCachedObservable()
    {
        var source = new TestReactiveObject();
        IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? observable = null;

        var first = ReactiveNotificationHelpers.GetChanging(source, ref observable);
        var second = ReactiveNotificationHelpers.GetChanging(source, ref observable);

        await Assert.That(second).IsSameReferenceAs(first);
    }

    /// <summary>Verifies that the property-changed observable is cached after its first creation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetChanged_RepeatedCalls_ReturnsCachedObservable()
    {
        var source = new TestReactiveObject();
        IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>>? observable = null;

        var first = ReactiveNotificationHelpers.GetChanged(source, ref observable);
        var second = ReactiveNotificationHelpers.GetChanged(source, ref observable);

        await Assert.That(second).IsSameReferenceAs(first);
    }

    /// <summary>Verifies that the exception observable is cached after its first creation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetThrownExceptions_RepeatedCalls_ReturnsCachedObservable()
    {
        var source = new TestReactiveObject();
        IObservable<Exception>? observable = null;

        var first = ReactiveNotificationHelpers.GetThrownExceptions(source, ref observable);
        var second = ReactiveNotificationHelpers.GetThrownExceptions(source, ref observable);

        await Assert.That(second).IsSameReferenceAs(first);
    }

    /// <summary>Minimal reactive object used to exercise helper-owned state.</summary>
    private sealed class TestReactiveObject : IReactiveObject
    {
        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets or sets the test value.</summary>
        public int Value { get; set; }

        /// <inheritdoc/>
        public void RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

        /// <inheritdoc/>
        public void RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    }
}
