// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the CreateCommand binding.
/// </summary>
public class CreatesCommandBindingTests
{
    /// <summary>
    /// Test that makes sure events binder binds to explicit event.
    /// </summary>
    [Fact]
    public void EventBinderBindsToExplicitEvent()
    {
        var input = new TestFixture();
        var fixture = new CreatesCommandBindingViaEvent();
        var wasCalled = false;
        var cmd = ReactiveCommand.Create<int>(_ => wasCalled = true);

        Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
        Assert.False(fixture.GetAffinityForObject(input.GetType(), false) > 0);

        var disposable = fixture.BindCommandToObject<PropertyChangedEventArgs>(cmd, input, Observable.Return((object)5), "PropertyChanged");
        input.IsNotNullString = "Foo";
        Assert.True(wasCalled);

        wasCalled = false;
        disposable?.Dispose();
        input.IsNotNullString = "Bar";
        Assert.False(wasCalled);
    }
}
