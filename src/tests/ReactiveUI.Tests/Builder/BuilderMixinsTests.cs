// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Tests.Builder;

/// <summary>Tests for the fluent converter/registration extension members on <see cref="BuilderMixins"/>.</summary>
public class BuilderMixinsTests
{
    /// <summary>The four <c>WithConverter</c> overloads and <c>WithConverters</c> each register on and return the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithConverterOverloadsReturnSameBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var afterTyped = BuilderMixins.WithConverter<string, int>(builder, new StubTypedConverter<string, int>());
        var afterInterface = BuilderMixins.WithConverter(builder, (IBindingTypeConverter)new StubTypedConverter<string, int>());
        var afterTypedFactory = BuilderMixins.WithConverter<string, int>(builder, static () => new StubTypedConverter<string, int>());
        var afterInterfaceFactory = BuilderMixins.WithConverter(builder, static () => (IBindingTypeConverter)new StubTypedConverter<string, int>());
        var afterMany = BuilderMixins.WithConverters(builder, new StubTypedConverter<string, int>(), new StubTypedConverter<string, int>());

        using (Assert.Multiple())
        {
            await Assert.That(afterTyped).IsSameReferenceAs(builder);
            await Assert.That(afterInterface).IsSameReferenceAs(builder);
            await Assert.That(afterTypedFactory).IsSameReferenceAs(builder);
            await Assert.That(afterInterfaceFactory).IsSameReferenceAs(builder);
            await Assert.That(afterMany).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Both <c>WithFallbackConverter</c> overloads register on and return the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithFallbackConverterOverloadsReturnSameBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var afterInstance = BuilderMixins.WithFallbackConverter(builder, new StubFallbackConverter());
        var afterFactory = BuilderMixins.WithFallbackConverter(builder, static () => (IBindingFallbackConverter)new StubFallbackConverter());

        using (Assert.Multiple())
        {
            await Assert.That(afterInstance).IsSameReferenceAs(builder);
            await Assert.That(afterFactory).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Both <c>WithSetMethodConverter</c> overloads register on and return the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithSetMethodConverterOverloadsReturnSameBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var afterInstance = BuilderMixins.WithSetMethodConverter(builder, new StubSetMethodConverter());
        var afterFactory = BuilderMixins.WithSetMethodConverter(builder, static () => (ISetMethodBindingConverter)new StubSetMethodConverter());

        using (Assert.Multiple())
        {
            await Assert.That(afterInstance).IsSameReferenceAs(builder);
            await Assert.That(afterFactory).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Importing converters from a resolver returns the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithConvertersFromReturnsSameBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = BuilderMixins.WithConvertersFrom(builder, new ModernDependencyResolver());

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Configuring the message bus and registering a constant view model each return the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMessageBusAndRegisterConstantViewModelReturnSameBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var afterBus = BuilderMixins.WithMessageBus(builder);
        var afterViewModel = BuilderMixins.RegisterConstantViewModel<StubViewModel>(builder);

        using (Assert.Multiple())
        {
            await Assert.That(afterBus).IsSameReferenceAs(builder);
            await Assert.That(afterViewModel).IsSameReferenceAs(builder);
        }
    }

    /// <summary>A typed converter stub that performs no conversion.</summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    private sealed class StubTypedConverter<TFrom, TTo> : BindingTypeConverter<TFrom, TTo>
    {
        /// <inheritdoc/>
        public override int GetAffinityForObjects() => 0;

        /// <inheritdoc/>
        public override bool TryConvert(TFrom? from, object? conversionHint, [NotNullWhen(true)] out TTo? result)
        {
            result = default;
            return false;
        }
    }

    /// <summary>A fallback converter stub that performs no conversion.</summary>
    private sealed class StubFallbackConverter : IBindingFallbackConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type fromType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type toType) => 0;

        /// <inheritdoc/>
        public bool TryConvert(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type fromType,
            object from,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            Type toType,
            object? conversionHint,
            [NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }

    /// <summary>A set-method converter stub that echoes the supplied value.</summary>
    private sealed class StubSetMethodConverter : ISetMethodBindingConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type? fromType, Type? toType) => 0;

        /// <inheritdoc/>
        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => newValue;
    }

    /// <summary>A minimal reactive view model used for constant registration.</summary>
    private sealed class StubViewModel : ReactiveObject;
}
