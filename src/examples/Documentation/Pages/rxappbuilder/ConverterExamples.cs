// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Shows every way to register a binding converter with the builder. Each call reaches the builder's <see
/// cref="ReactiveUIBuilder.ConverterService"/> immediately, so nothing here needs <c>BuildApp</c>.
/// </summary>
public static class ConverterExamples
{
    /// <summary>Registers typed, fallback and set-method converters, then reads each one back from the converter service.</summary>
    public static void ConfigureBindingConverters()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = resolver.CreateReactiveUIBuilder();

        IBindingTypeConverter servingsAsPlainConverter = new ServingsConverter();

        _ = builder
            .WithConverter(new CookTimeConverter())
            .WithConverter(servingsAsPlainConverter)
            .WithConverter(static () => new CookTimeConverter())
            .WithConverter(static () => (IBindingTypeConverter)new ServingsConverter())
            .WithConverters(new CookTimeConverter(), new ServingsConverter())
            .WithFallbackConverter(new SpiceLevelFallbackConverter())
            .WithFallbackConverter(static () => new SpiceLevelFallbackConverter())
            .WithSetMethodConverter(new IngredientSetConverter())
            .WithSetMethodConverter(static () => new IngredientSetConverter());

        Console.WriteLine(builder.ConverterService.ResolveConverter(typeof(TimeSpan), typeof(string))?.GetType().Name);
        Console.WriteLine(builder.ConverterService.ResolveConverter(typeof(int), typeof(string))?.GetType().Name);
        Console.WriteLine(builder.ConverterService.ResolveConverter(typeof(byte), typeof(string))?.GetType().Name);
        Console.WriteLine(builder.ConverterService.ResolveSetMethodConverter(typeof(string), typeof(List<string>))?.GetType().Name);

        // Output:
        // CookTimeConverter
        // ServingsConverter
        // SpiceLevelFallbackConverter
        // IngredientSetConverter
    }

    /// <summary><c>WithConvertersFrom</c> imports every converter already registered with another resolver.</summary>
    public static void ImportConvertersFromAnotherResolver()
    {
        using ModernDependencyResolver sourceResolver = new();
        sourceResolver.RegisterConstant<IBindingTypeConverter>(new OvenTemperatureConverter());

        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = resolver.CreateReactiveUIBuilder();

        Console.WriteLine(builder.ConverterService.ResolveConverter(typeof(double), typeof(string)) is null);

        _ = builder.WithConvertersFrom(sourceResolver);

        Console.WriteLine(builder.ConverterService.ResolveConverter(typeof(double), typeof(string))?.GetType().Name);

        // Output:
        // True
        // OvenTemperatureConverter
    }
}
