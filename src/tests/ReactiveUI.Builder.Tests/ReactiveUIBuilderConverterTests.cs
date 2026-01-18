// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests for ReactiveUIBuilder converter registration methods.
/// </summary>
[NotInParallel]
public class ReactiveUIBuilderConverterTests
{
    [Before(Test)]
    public void SetUp() => AppBuilder.ResetBuilderStateForTests();

    [Test]
    public async Task WithConverter_TypedConverter_ReturnsBuilderForChaining()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        var converter = new TestTypedConverter();

        var result = builder.WithConverter(converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public void WithConverter_TypedConverter_WithNull_Throws()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((BindingTypeConverter<int, string>)null!));
    }

    [Test]
    public async Task WithConverter_GenericInterface_ReturnsBuilderForChaining()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();
        var converter = new TestBindingConverter();

        var result = builder.WithConverter((IBindingTypeConverter)converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public void WithConverter_GenericInterface_WithNull_Throws()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((IBindingTypeConverter)null!));
    }

    [Test]
    public async Task WithConverter_TypedFactory_ReturnsBuilderForChaining()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        var result = builder.WithConverter<int, string>(() => new TestTypedConverter());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public void WithConverter_TypedFactory_WithNull_Throws()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter<int, string>((Func<BindingTypeConverter<int, string>>)null!));
    }

    [Test]
    public async Task WithConverter_InterfaceFactory_ReturnsBuilderForChaining()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        var result = builder.WithConverter(() => (IBindingTypeConverter)new TestBindingConverter());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public void WithConverter_InterfaceFactory_WithNull_Throws()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((Func<IBindingTypeConverter>)null!));
    }

    [Test]
    public void WithConvertersFrom_WithNull_Throws()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            builder.WithConvertersFrom(null!));
    }

    [Test]
    public async Task WithConvertersFrom_ReturnsBuilderForChaining()
    {
        using var sourceLocator = new ModernDependencyResolver();
        using var targetLocator = new ModernDependencyResolver();
        var builder = targetLocator.CreateReactiveUIBuilder();

        var result = builder.WithConvertersFrom(sourceLocator);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    private sealed class TestTypedConverter : BindingTypeConverter<int, string>
    {
        public override int GetAffinityForObjects() => 1;

        public override bool TryConvert(int from, object? conversionHint, [NotNullWhen(true)] out string? result)
        {
            result = from.ToString();
            return true;
        }
    }

    private sealed class TestBindingConverter : IBindingTypeConverter
    {
        public Type FromType => typeof(bool);

        public Type ToType => typeof(string);

        public int GetAffinityForObjects() => 1;

        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = from?.ToString();
            return from != null;
        }
    }
}