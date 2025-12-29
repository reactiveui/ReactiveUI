// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.PlatformServices;

namespace ReactiveUI.Blazor;

/// <summary>
/// Blazor Framework platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Register uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("Register uses methods that may require unreferenced code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Not all paths use reflection")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(registerFunction);
#else
        if (registerFunction is null)
        {
            throw new ArgumentNullException(nameof(registerFunction));
        }
#endif

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
        registerFunction(static () => new PlatformOperations(), typeof(IPlatformOperations));

        if (Type.GetType("Mono.Runtime") is not null)
        {
            PlatformEnlightenmentProvider.Current.EnableWasm();
        }

        RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
        RxSchedulers.MainThreadScheduler = CurrentThreadScheduler.Instance;
    }
}
