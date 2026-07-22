// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests for ReactiveUIBuilder converter registration methods.</summary>
[NotInParallel]
public class ReactiveUIBuilderConverterTests
{
    /// <summary>Verifies that registering a typed converter returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithConverter_TypedConverter_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var converter = new TestTypedConverter();

        var result = builder.WithConverter(converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null typed converter throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithConverter_TypedConverter_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((BindingTypeConverter<int, string>)null!));
    }

    /// <summary>Verifies that registering a converter via the interface returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithConverter_GenericInterface_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var converter = new TestBindingConverter();

        var result = builder.WithConverter((IBindingTypeConverter)converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null interface converter throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithConverter_GenericInterface_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((IBindingTypeConverter)null!));
    }

    /// <summary>Verifies that registering a typed converter factory returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithConverter_TypedFactory_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithConverter(static () => new TestTypedConverter());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null typed converter factory throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithConverter_TypedFactory_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((Func<BindingTypeConverter<int, string>>)null!));
    }

    /// <summary>Verifies that registering an interface converter factory returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithConverter_InterfaceFactory_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithConverter(static () => (IBindingTypeConverter)new TestBindingConverter());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null interface converter factory throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithConverter_InterfaceFactory_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithConverter((Func<IBindingTypeConverter>)null!));
    }

    /// <summary>Verifies that copying converters from a null resolver throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithConvertersFrom_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithConvertersFrom(null!));
    }

    /// <summary>Verifies that copying converters from a resolver returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithConvertersFrom_ReturnsBuilderForChaining()
    {
        using var sourceLocator = new ModernDependencyResolver();
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithConvertersFrom(sourceLocator);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a fallback converter instance returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithFallbackConverter_Instance_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        var converter = new TestFallbackConverter();

        var result = builder.WithFallbackConverter(converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null fallback converter instance throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithFallbackConverter_Instance_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithFallbackConverter((IBindingFallbackConverter)null!));
    }

    /// <summary>Verifies that registering a fallback converter factory returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithFallbackConverter_Factory_ReturnsBuilderForChaining()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithFallbackConverter(static () => new TestFallbackConverter());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that registering a null fallback converter factory throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithFallbackConverter_Factory_WithNull_Throws()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() =>
            builder.WithFallbackConverter((Func<IBindingFallbackConverter>)null!));
    }

    /// <summary>Verifies that calling WithFallbackConverter on an interface-typed builder does not recurse (regression for issue 4293).</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1859:Use concrete types when possible for improved performance",
        Justification = "Regression test must use the IReactiveUIBuilder interface type (issue #4293).")]
    public async Task WithFallbackConverter_ViaInterfaceTypedVariable_DoesNotRecurse()
    {
        // Regression test for https://github.com/reactiveui/ReactiveUI/issues/4293
        // Calling WithFallbackConverter on an IReactiveUIBuilder-typed variable caused
        // infinite recursion (StackOverflowException) because the extension method in
        // BuilderMixins called itself instead of delegating to the interface method.
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        var converter = new TestFallbackConverter();

        var result = builder.WithFallbackConverter(converter);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Test typed converter that converts an <see cref="int"/> to a <see cref="string"/>.</summary>
    private sealed class TestTypedConverter : BindingTypeConverter<int, string>
    {
        /// <inheritdoc/>
        public override int GetAffinityForObjects() => 1;

        /// <inheritdoc/>
        public override bool TryConvert(int from, object? conversionHint, [NotNullWhen(true)] out string? result)
        {
            result = from.ToString();
            return true;
        }
    }

    /// <summary>Test binding converter that converts a <see cref="bool"/> to a <see cref="string"/>.</summary>
    private sealed class TestBindingConverter : IBindingTypeConverter
    {
        /// <inheritdoc/>
        public Type FromType => typeof(bool);

        /// <inheritdoc/>
        public Type ToType => typeof(string);

        /// <inheritdoc/>
        public int GetAffinityForObjects() => 1;

        /// <inheritdoc/>
        public bool TryConvertTyped(object? from, object? conversionHint, out object? result)
        {
            result = from?.ToString();
            return from is not null;
        }
    }

    /// <summary>Test fallback converter that passes the source value through unchanged.</summary>
    private sealed class TestFallbackConverter : IBindingFallbackConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => 1;

        /// <inheritdoc/>
        public bool TryConvert(
            Type fromType,
            object from,
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            result = from;
            return true;
        }
    }
}
