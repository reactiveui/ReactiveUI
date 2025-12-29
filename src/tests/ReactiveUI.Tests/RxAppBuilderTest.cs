// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="RxAppBuilder"/>.
/// </summary>
public class RxAppBuilderTest
{
    /// <summary>
    /// Tests that CreateReactiveUIBuilder returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_ReturnsBuilder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    /// Tests that CreateReactiveUIBuilder with resolver returns a builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_WithResolver_ReturnsBuilder()
    {
        var resolver = new TestResolver();

        var builder = resolver.CreateReactiveUIBuilder();

        await Assert.That(builder).IsNotNull();
        await Assert.That(builder).IsTypeOf<ReactiveUIBuilder>();
    }

    /// <summary>
    /// Tests that CreateReactiveUIBuilder throws for null resolver.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateReactiveUIBuilder_NullResolver_Throws()
    {
        IMutableDependencyResolver resolver = null!;

        await Assert.That(() => resolver.CreateReactiveUIBuilder())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test resolver for testing.
    /// </summary>
    private class TestResolver : IMutableDependencyResolver, IReadonlyDependencyResolver
    {
        public object? GetService(Type? serviceType, string? contract = null) => null;

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null) => [];

        public bool HasRegistration(Type? serviceType, string? contract = null) => false;

        public void Register(Func<object?> factory, Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterCurrent(Type? serviceType, string? contract = null)
        {
        }

        public void UnregisterAll(Type? serviceType, string? contract = null)
        {
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string? contract, Action<IDisposable> callback) => Disposable.Empty;
    }
}
