// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class CreatesCommandBindingTests
{
    /// <summary>
    /// Test that makes sure events binder binds to explicit event.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task EventBinderBindsToExplicitEvent()
    {
        var input = new TestFixture();
        var fixture = new CreatesCommandBindingViaEvent();
        var wasCalled = false;
        var cmd = ReactiveCommand.Create<int>(_ => wasCalled = true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject<TestFixture>(hasEventTarget: true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject<TestFixture>(hasEventTarget: false)).IsLessThanOrEqualTo(0);
        }

        var disposable = fixture.BindCommandToObject<TestFixture, PropertyChangedEventArgs>(cmd, input, Observable.Return((object)5), "PropertyChanged");
        input.IsNotNullString = "Foo";
        await Assert.That(wasCalled).IsTrue();

        wasCalled = false;
        disposable?.Dispose();
        input.IsNotNullString = "Bar";
        await Assert.That(wasCalled).IsFalse();
    }
}
