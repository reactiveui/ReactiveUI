// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the CreateCommand binding.
/// </summary>
[TestFixture]
public class CreatesCommandBindingTests
{
    /// <summary>
    /// Test that makes sure events binder binds to explicit event.
    /// </summary>
    [Test]
    public void EventBinderBindsToExplicitEvent()
    {
        var input = new TestFixture();
        var fixture = new CreatesCommandBindingViaEvent();
        var wasCalled = false;
        var cmd = ReactiveCommand.Create<int>(_ => wasCalled = true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.GetAffinityForObject(input.GetType(), true), Is.GreaterThan(0));
            Assert.That(fixture.GetAffinityForObject(input.GetType(), false), Is.LessThanOrEqualTo(0));
        }

        var disposable = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object)5), "PropertyChanged");
        input.IsNotNullString = "Foo";
        Assert.That(wasCalled, Is.True);

        wasCalled = false;
        disposable?.Dispose();
        input.IsNotNullString = "Bar";
        Assert.That(wasCalled, Is.False);
    }
}
