// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using Splat.Builder;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="WpfReactiveUIBuilderExtensions"/>.
/// </summary>
[NotInParallel]
public class WpfReactiveUIBuilderExtensionsTest
{
    /// <summary>
    /// Tests that WpfMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_IsNotNull()
    {
        await Assert.That(WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler).IsNotNull();
    }

    /// <summary>
    /// Tests that WithWpf throws when builder is null.
    /// </summary>
    [Test]
    public void WithWpf_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WpfReactiveUIBuilderExtensions.WithWpf(null!));
    }

    /// <summary>
    /// Tests that WithWpf configures builder correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_ConfiguresBuilder()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWpf();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>
    /// Tests that WithWpfScheduler throws when builder is null.
    /// </summary>
    [Test]
    public void WithWpfScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WpfReactiveUIBuilderExtensions.WithWpfScheduler(null!));
    }

    /// <summary>
    /// Tests that WithWpfScheduler configures scheduler correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpfScheduler_ConfiguresScheduler()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWpfScheduler();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>
    /// Tests that WithWpfConverters throws when builder is null.
    /// </summary>
    [Test]
    public void WithWpfConverters_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WpfReactiveUIBuilderExtensions.WithWpfConverters(null!));
    }

    /// <summary>
    /// Tests that WithWpfConverters registers WPF-specific converters in the ConverterService.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpfConverters_RegistersWpfSpecificConverters()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)resolver.CreateReactiveUIBuilder();

            builder.WithWpfConverters();

            // Verify BooleanToVisibilityTypeConverter is registered
            var boolToVisibility = builder.ConverterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility));
            await Assert.That(boolToVisibility).IsNotNull();
            await Assert.That(boolToVisibility).IsTypeOf<BooleanToVisibilityTypeConverter>();

            // Verify VisibilityToBooleanTypeConverter is registered
            var visibilityToBool = builder.ConverterService.TypedConverters.TryGetConverter(typeof(Visibility), typeof(bool));
            await Assert.That(visibilityToBool).IsNotNull();
            await Assert.That(visibilityToBool).IsTypeOf<VisibilityToBooleanTypeConverter>();

            // Verify ComponentModelFallbackConverter is registered as a fallback converter
            var fallbackConverters = builder.ConverterService.FallbackConverters.GetAllConverters().ToList();
            await Assert.That(fallbackConverters).IsNotEmpty();
            await Assert.That(fallbackConverters.OfType<ComponentModelFallbackConverter>().Any()).IsTrue();
        }
    }

    /// <summary>
    /// Tests that WithWpf registers all required converters to the ConverterService via BuildApp.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_BuildApp_RegistersAllConvertersToConverterService()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)AppLocator.CurrentMutable.CreateReactiveUIBuilder();
            builder.WithWpf().BuildApp();

            var converterService = builder.ConverterService;

            // WPF-specific converters
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(Visibility), typeof(bool))).IsNotNull();

            // Standard converters from WithCoreServices
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(int), typeof(string))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(string), typeof(int))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(bool), typeof(string))).IsNotNull();

            // Fallback converter
            var fallbackConverters = converterService.FallbackConverters.GetAllConverters().ToList();
            await Assert.That(fallbackConverters).IsNotEmpty();
            await Assert.That(fallbackConverters.OfType<ComponentModelFallbackConverter>().Any()).IsTrue();
        }
    }

    /// <summary>
    /// Tests that after WithWpf, the BooleanToVisibilityTypeConverter correctly converts values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_BoolToVisibilityConverter_ConvertsCorrectly()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)AppLocator.CurrentMutable.CreateReactiveUIBuilder();
            builder.WithWpf().BuildApp();

            var converter = builder.ConverterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility));
            await Assert.That(converter).IsNotNull();

            var success = converter!.TryConvertTyped(true, null, out var result);
            await Assert.That(success).IsTrue();
            await Assert.That(result).IsEqualTo(Visibility.Visible);
        }
    }
}
