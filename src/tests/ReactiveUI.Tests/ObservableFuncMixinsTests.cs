// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Tests.WhenAny;

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="ObservableFuncMixins"/>, which converts a property expression into an observable sequence.</summary>
public class ObservableFuncMixinsTests
{
    /// <summary>The value a property holds at subscription time.</summary>
    private const string Initial = "initial";

    /// <summary>The value a property is changed to after subscription.</summary>
    private const string Changed = "changed";

    /// <summary>Verifies the single-argument overload emits the initial value and subsequent property changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToObservable_EmitsInitialAndChanges()
    {
        var vm = new WhenAnyArityTestViewModel { Property1 = Initial };
        Expression<Func<WhenAnyArityTestViewModel, string?>> expression = x => x.Property1;
        var results = new List<string?>();

        using var sub = expression.ToObservable(vm).Subscribe(results.Add);
        vm.Property1 = Changed;

        string?[] expected = [Initial, Changed];
        await Assert.That(results).IsEquivalentTo(expected);
    }

    /// <summary>Verifies the skip-initial overload omits the value present at subscription time.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToObservable_SkipInitial_OmitsFirstValue()
    {
        var vm = new WhenAnyArityTestViewModel { Property1 = Initial };
        Expression<Func<WhenAnyArityTestViewModel, string?>> expression = x => x.Property1;
        var results = new List<string?>();

        using var sub = expression.ToObservable(vm, false, true).Subscribe(results.Add);
        vm.Property1 = Changed;

        string?[] expected = [Changed];
        await Assert.That(results).IsEquivalentTo(expected);
    }

    /// <summary>Verifies the before-change overload emits the value that precedes each change.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToObservable_BeforeChange_EmitsPreviousValues()
    {
        var vm = new WhenAnyArityTestViewModel { Property1 = Initial };
        Expression<Func<WhenAnyArityTestViewModel, string?>> expression = x => x.Property1;
        var results = new List<string?>();

        using var sub = expression.ToObservable(vm, true).Subscribe(results.Add);
        vm.Property1 = Changed;

        await Assert.That(results).Contains(Initial);
    }

    /// <summary>Verifies a null expression is rejected.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToObservable_NullExpression_Throws() =>
        await Assert.That(static () => ((Expression<Func<WhenAnyArityTestViewModel, string?>>)null!).ToObservable(new()))
            .Throws<ArgumentNullException>();
}
