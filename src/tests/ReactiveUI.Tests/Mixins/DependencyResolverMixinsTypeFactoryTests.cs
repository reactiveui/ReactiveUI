// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ReactiveUI.Tests.Mixins;

/// <summary>Tests for the internal <see cref="DependencyResolverMixins"/> type factory.</summary>
public class DependencyResolverMixinsTypeFactoryTests
{
    /// <summary>A type with a public parameterless constructor yields a factory that creates an instance.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeFactoryCreatesInstanceForParameterlessType()
    {
        var factory = DependencyResolverMixins.TypeFactory(typeof(ParameterlessType).GetTypeInfo());

        var instance = factory();

        using (Assert.Multiple())
        {
            await Assert.That(instance).IsTypeOf<ParameterlessType>();
            await Assert.That(((ParameterlessType)instance).Initialized).IsTrue();
        }
    }

    /// <summary>A type lacking a public parameterless constructor throws when its factory is built.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeFactoryThrowsForTypeMissingParameterlessConstructor() =>
        await Assert.That(static () => DependencyResolverMixins.TypeFactory(typeof(NoParameterlessType).GetTypeInfo()))
            .Throws<InvalidOperationException>();

    /// <summary>A type with a public parameterless constructor.</summary>
    private sealed class ParameterlessType
    {
        /// <summary>Gets a value indicating whether the instance was constructed.</summary>
        public bool Initialized { get; } = true;
    }

    /// <summary>A type whose only constructor requires an argument.</summary>
    /// <param name="value">An unused required argument.</param>
    private sealed class NoParameterlessType(int value)
    {
        /// <summary>Gets the captured value.</summary>
        [SuppressMessage(
            "Design",
            "SST2324:public member on non-public type",
            Justification = "the public surface mirrors the sibling test fixture's shape for readability; " +
                "the containing test double is an intentionally non-public detail.")]
        public int Value { get; } = value;
    }
}
