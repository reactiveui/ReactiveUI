// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// The main registration for common classes for the Splat dependency injection.
/// We have code that runs reflection through the different ReactiveUI classes
/// searching for IWantsToRegisterStuff and will register all our required DI
/// interfaces. The registered items in this classes are common for all Platforms.
/// To get these registrations after the main ReactiveUI Initialization use the
/// DependencyResolverMixins.InitializeReactiveUI() extension method.
/// </summary>
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new INPCObservableForProperty());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new IROObservableForProperty());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new POCOObservableForProperty());

        // General converters - register to Splat for backward compatibility
        RegisterConverter(registrar, new EqualityTypeConverter());
        RegisterConverter(registrar, new StringConverter());

        // Numeric → String converters
        RegisterConverter(registrar, new ByteToStringTypeConverter());
        RegisterConverter(registrar, new NullableByteToStringTypeConverter());
        RegisterConverter(registrar, new ShortToStringTypeConverter());
        RegisterConverter(registrar, new NullableShortToStringTypeConverter());
        RegisterConverter(registrar, new IntegerToStringTypeConverter());
        RegisterConverter(registrar, new NullableIntegerToStringTypeConverter());
        RegisterConverter(registrar, new LongToStringTypeConverter());
        RegisterConverter(registrar, new NullableLongToStringTypeConverter());
        RegisterConverter(registrar, new SingleToStringTypeConverter());
        RegisterConverter(registrar, new NullableSingleToStringTypeConverter());
        RegisterConverter(registrar, new DoubleToStringTypeConverter());
        RegisterConverter(registrar, new NullableDoubleToStringTypeConverter());
        RegisterConverter(registrar, new DecimalToStringTypeConverter());
        RegisterConverter(registrar, new NullableDecimalToStringTypeConverter());

        // String → Numeric converters
        RegisterConverter(registrar, new StringToByteTypeConverter());
        RegisterConverter(registrar, new StringToNullableByteTypeConverter());
        RegisterConverter(registrar, new StringToShortTypeConverter());
        RegisterConverter(registrar, new StringToNullableShortTypeConverter());
        RegisterConverter(registrar, new StringToIntegerTypeConverter());
        RegisterConverter(registrar, new StringToNullableIntegerTypeConverter());
        RegisterConverter(registrar, new StringToLongTypeConverter());
        RegisterConverter(registrar, new StringToNullableLongTypeConverter());
        RegisterConverter(registrar, new StringToSingleTypeConverter());
        RegisterConverter(registrar, new StringToNullableSingleTypeConverter());
        RegisterConverter(registrar, new StringToDoubleTypeConverter());
        RegisterConverter(registrar, new StringToNullableDoubleTypeConverter());
        RegisterConverter(registrar, new StringToDecimalTypeConverter());
        RegisterConverter(registrar, new StringToNullableDecimalTypeConverter());

        // Boolean ↔ String converters
        RegisterConverter(registrar, new BooleanToStringTypeConverter());
        RegisterConverter(registrar, new NullableBooleanToStringTypeConverter());
        RegisterConverter(registrar, new StringToBooleanTypeConverter());
        RegisterConverter(registrar, new StringToNullableBooleanTypeConverter());

        // Guid ↔ String converters
        RegisterConverter(registrar, new GuidToStringTypeConverter());
        RegisterConverter(registrar, new NullableGuidToStringTypeConverter());
        RegisterConverter(registrar, new StringToGuidTypeConverter());
        RegisterConverter(registrar, new StringToNullableGuidTypeConverter());

        // DateTime ↔ String converters
        RegisterConverter(registrar, new DateTimeToStringTypeConverter());
        RegisterConverter(registrar, new NullableDateTimeToStringTypeConverter());
        RegisterConverter(registrar, new StringToDateTimeTypeConverter());
        RegisterConverter(registrar, new StringToNullableDateTimeTypeConverter());

        // DateTimeOffset ↔ String converters
        RegisterConverter(registrar, new DateTimeOffsetToStringTypeConverter());
        RegisterConverter(registrar, new NullableDateTimeOffsetToStringTypeConverter());
        RegisterConverter(registrar, new StringToDateTimeOffsetTypeConverter());
        RegisterConverter(registrar, new StringToNullableDateTimeOffsetTypeConverter());

        // TimeSpan ↔ String converters
        RegisterConverter(registrar, new TimeSpanToStringTypeConverter());
        RegisterConverter(registrar, new NullableTimeSpanToStringTypeConverter());
        RegisterConverter(registrar, new StringToTimeSpanTypeConverter());
        RegisterConverter(registrar, new StringToNullableTimeSpanTypeConverter());

#if NET6_0_OR_GREATER
        // DateOnly ↔ String converters (.NET 6+)
        RegisterConverter(registrar, new DateOnlyToStringTypeConverter());
        RegisterConverter(registrar, new NullableDateOnlyToStringTypeConverter());
        RegisterConverter(registrar, new StringToDateOnlyTypeConverter());
        RegisterConverter(registrar, new StringToNullableDateOnlyTypeConverter());

        // TimeOnly ↔ String converters (.NET 6+)
        RegisterConverter(registrar, new TimeOnlyToStringTypeConverter());
        RegisterConverter(registrar, new NullableTimeOnlyToStringTypeConverter());
        RegisterConverter(registrar, new StringToTimeOnlyTypeConverter());
        RegisterConverter(registrar, new StringToNullableTimeOnlyTypeConverter());
#endif

        // Uri ↔ String converters
        RegisterConverter(registrar, new UriToStringTypeConverter());
        RegisterConverter(registrar, new StringToUriTypeConverter());

        // Nullable ↔ Non-Nullable converters
        RegisterUnidirectionalConverter<byte, byte?, ByteToNullableByteTypeConverter>(registrar);
        RegisterUnidirectionalConverter<byte?, byte, NullableByteToByteTypeConverter>(registrar);
        RegisterUnidirectionalConverter<short, short?, ShortToNullableShortTypeConverter>(registrar);
        RegisterUnidirectionalConverter<short?, short, NullableShortToShortTypeConverter>(registrar);
        RegisterUnidirectionalConverter<int, int?, IntegerToNullableIntegerTypeConverter>(registrar);
        RegisterUnidirectionalConverter<int?, int, NullableIntegerToIntegerTypeConverter>(registrar);
        RegisterUnidirectionalConverter<long, long?, LongToNullableLongTypeConverter>(registrar);
        RegisterUnidirectionalConverter<long?, long, NullableLongToLongTypeConverter>(registrar);
        RegisterUnidirectionalConverter<float, float?, SingleToNullableSingleTypeConverter>(registrar);
        RegisterUnidirectionalConverter<float?, float, NullableSingleToSingleTypeConverter>(registrar);
        RegisterUnidirectionalConverter<double, double?, DoubleToNullableDoubleTypeConverter>(registrar);
        RegisterUnidirectionalConverter<double?, double, NullableDoubleToDoubleTypeConverter>(registrar);
        RegisterUnidirectionalConverter<decimal, decimal?, DecimalToNullableDecimalTypeConverter>(registrar);
        RegisterUnidirectionalConverter<decimal?, decimal, NullableDecimalToDecimalTypeConverter>(registrar);

        registrar.RegisterConstant<IViewLocator>(static () => new DefaultViewLocator());
        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new CanActivateViewFetcher());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new CreatesCommandBindingViaEvent());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new CreatesCommandBindingViaCommandParameter());
    }

    /// <summary>
    /// Helper method to register a converter to Splat for backward compatibility.
    /// </summary>
    /// <param name="registrar">The Splat registrar.</param>
    /// <param name="converter">The converter instance to register.</param>
    /// <remarks>
    /// This registers converters to Splat for backward compatibility.
    /// When using ReactiveUIBuilder, converters are also registered to the
    /// ConverterService separately through the builder's initialization.
    /// </remarks>
    private static void RegisterConverter(
        IRegistrar registrar,
        IBindingTypeConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);
        ArgumentExceptionHelper.ThrowIfNull(converter);

        // Register to Splat for backward compatibility
        registrar.RegisterConstant<IBindingTypeConverter>(() => converter);
    }

    /// <summary>
    /// Helper method to register a unidirectional type converter with explicit generic instantiation.
    /// </summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    /// <typeparam name="TConverter">The converter type that handles TFrom→TTo conversion.</typeparam>
    /// <param name="registrar">The dependency resolver to register with.</param>
    /// <remarks>
    /// This method registers the converter twice:
    /// <list type="bullet">
    /// <item><description>As <see cref="IBindingTypeConverter{TFrom, TTo}"/> for typed lookup</description></item>
    /// <item><description>As <see cref="IBindingTypeConverter"/> for affinity-based discovery</description></item>
    /// </list>
    /// </remarks>
    private static void RegisterUnidirectionalConverter<TFrom, TTo, TConverter>(
        IRegistrar registrar)
        where TConverter : IBindingTypeConverter<TFrom, TTo>, new()
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        var instance = new TConverter();

        // Register typed interface for direct type lookup
        registrar.RegisterConstant<IBindingTypeConverter<TFrom, TTo>>(() => instance);

        // Register base interface for affinity-based discovery
        registrar.RegisterConstant<IBindingTypeConverter>(() => instance);
    }

    /// <summary>
    /// Helper method to register a bidirectional type converter with explicit generic instantiations.
    /// </summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The target type.</typeparam>
    /// <typeparam name="TConverter">The converter type that handles both TFrom→TTo and TTo→TFrom conversions.</typeparam>
    /// <param name="registrar">The dependency resolver to register with.</param>
    /// <remarks>
    /// This method registers the converter three times:
    /// <list type="bullet">
    /// <item><description>As <see cref="IBindingTypeConverter{TFrom, TTo}"/> for TFrom→TTo conversion</description></item>
    /// <item><description>As <see cref="IBindingTypeConverter{TTo, TFrom}"/> for TTo→TFrom conversion</description></item>
    /// <item><description>As <see cref="IBindingTypeConverter"/> for affinity-based discovery</description></item>
    /// </list>
    /// </remarks>
    private static void RegisterBidirectionalConverter<TFrom, TTo, TConverter>(
        IRegistrar registrar)
        where TConverter : IBindingTypeConverter<TFrom, TTo>, IBindingTypeConverter<TTo, TFrom>, new()
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        var instance = new TConverter();

        // Register both generic directions
        registrar.Register<IBindingTypeConverter<TFrom, TTo>>(() => instance);
        registrar.Register<IBindingTypeConverter<TTo, TFrom>>(() => instance);

        // Register base interface for affinity-based discovery
        registrar.RegisterConstant<IBindingTypeConverter>(() => instance);
    }
}
