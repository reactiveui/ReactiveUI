// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using Splat;

namespace ReactiveUI.Tests.Commands;

/// <summary>Tests for command binding creation.</summary>
public class CreatesCommandBindingTests
{
    /// <summary>Binding throws when no command binder is registered for the target type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [NotInParallel]
    public async Task BindCommandToObjectThrowsWhenNoBinderFound()
    {
        using var locator = new ModernDependencyResolver();
        using (locator.WithResolver())
        {
            await Assert.That(() => Bind(new TestFixture())).Throws<InvalidOperationException>();
        }
    }

    /// <summary>Test that makes sure events binder binds to explicit event.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task EventBinderBindsToExplicitEvent()
    {
        var input = new TestFixture();
        var fixture = new CreatesCommandBindingViaEvent();
        var wasCalled = false;
        var cmd = ReactiveCommand.Create<int>(_ => wasCalled = true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject<TestFixture>(true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject<TestFixture>(false)).IsLessThanOrEqualTo(0);
        }

        var disposable = fixture.BindCommandToObject<TestFixture, PropertyChangedEventArgs>(
            cmd,
            input,
            Signal.Emit((object)5),
            "PropertyChanged");
        input.IsNotNullString = "Foo";
        await Assert.That(wasCalled).IsTrue();

        wasCalled = false;
        disposable?.Dispose();
        input.IsNotNullString = "Bar";
        await Assert.That(wasCalled).IsFalse();
    }

    /// <summary>Invokes the default command binder lookup for the supplied target.</summary>
    /// <param name="target">The target to bind the command to.</param>
    /// <returns>The binding disposable.</returns>
    [RequiresUnreferencedCode("Exercises the reflection-based command binder lookup.")]
    private static IDisposable Bind(TestFixture target) =>
        CreatesCommandBinding.BindCommandToObject(ReactiveCommand.Create(() => { }), target, Signal.Silent<object?>());
}
