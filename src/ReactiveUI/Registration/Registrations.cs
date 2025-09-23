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
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Does not use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Does not use reflection")]
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        registerFunction.ArgumentNullExceptionThrowIfNull(nameof(registerFunction));

        registerFunction(static () => new INPCObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new IROObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new POCOObservableForProperty(), typeof(ICreatesObservableForProperty));
        registerFunction(static () => new EqualityTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new ByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new ShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new IntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableIntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new LongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableLongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableSingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableDoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new NullableDecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(static () => new DefaultViewLocator(), typeof(IViewLocator));
        registerFunction(static () => new CanActivateViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(static () => new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));
        registerFunction(static () => new CreatesCommandBindingViaCommandParameter(), typeof(ICreatesCommandBinding));
    }
}
