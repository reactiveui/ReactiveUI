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

        // General converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new EqualityTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringConverter());

        // Numeric → String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new IntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableIntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new LongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableLongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new SingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableSingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDecimalToStringTypeConverter());

        // String → Numeric converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToByteTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableByteTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToShortTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableShortTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToIntegerTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableIntegerTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToLongTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableLongTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToSingleTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableSingleTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToDoubleTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableDoubleTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToDecimalTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableDecimalTypeConverter());

        // Boolean ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new BooleanToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableBooleanToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToBooleanTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableBooleanTypeConverter());

        // Guid ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new GuidToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableGuidToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToGuidTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableGuidTypeConverter());

        // DateTime ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DateTimeToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDateTimeToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToDateTimeTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableDateTimeTypeConverter());

        // DateTimeOffset ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DateTimeOffsetToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDateTimeOffsetToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToDateTimeOffsetTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableDateTimeOffsetTypeConverter());

        // TimeSpan ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new TimeSpanToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableTimeSpanToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToTimeSpanTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableTimeSpanTypeConverter());

#if NET6_0_OR_GREATER
        // DateOnly ↔ String converters (.NET 6+)
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DateOnlyToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDateOnlyToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToDateOnlyTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableDateOnlyTypeConverter());

        // TimeOnly ↔ String converters (.NET 6+)
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new TimeOnlyToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableTimeOnlyToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToTimeOnlyTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToNullableTimeOnlyTypeConverter());
#endif

        // Uri ↔ String converters
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new UriToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringToUriTypeConverter());

        registrar.RegisterConstant<IViewLocator>(static () => new DefaultViewLocator());
        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new CanActivateViewFetcher());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new CreatesCommandBindingViaEvent());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new CreatesCommandBindingViaCommandParameter());
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
