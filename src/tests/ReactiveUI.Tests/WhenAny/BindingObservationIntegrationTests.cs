// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Exercises Binding observation when a property path is supplied through a generic helper.</summary>
public class BindingObservationIntegrationTests
{
    /// <summary>The expected count after one property change.</summary>
    private const int ExpectedCountTwo = 2;

    /// <summary>Verifies the Unsafe path observes an expression the generator cannot read at the call site.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValueUnsafe_GenericHelper_DeliversInitialAndChanges()
    {
        var viewModel = new WhenAnyArityTestViewModel();
        Expression<Func<WhenAnyArityTestViewModel, string?>> property = static x => x.Property1;
        var values = new List<string?>();

        using var subscription = ObserveFromGenericHelper(viewModel, property).Subscribe(values.Add);
        await Assert.That(values).Count().IsEqualTo(1);
        await Assert.That(values[0]).IsNull();

        viewModel.Property1 = "updated";

        await Assert.That(values).Count().IsEqualTo(ExpectedCountTwo);
        await Assert.That(values[1]).IsEqualTo("updated");
    }

    /// <summary>Resolves a property path supplied through generic type parameters at runtime.</summary>
    /// <typeparam name="TObject">The source object type.</typeparam>
    /// <typeparam name="TValue">The selected property type.</typeparam>
    /// <param name="source">The object to observe.</param>
    /// <param name="property">The selected property.</param>
    /// <returns>The property's current and future values.</returns>
    private static IObservable<TValue> ObserveFromGenericHelper<TObject, TValue>(
        TObject source,
        Expression<Func<TObject, TValue>> property)
        where TObject : class => source.WhenAnyValueUnsafe(property);
}
