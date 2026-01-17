// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
#if !XAMARINIOS && !XAMARINMAC && !XAMARINTVOS && !MONOANDROID
using System.ComponentModel.DataAnnotations;
#endif

namespace ReactiveUI;

/// <summary>
/// Reactive Property Extensions.
/// </summary>
public static class ReactivePropertyMixins
{
#if !XAMARINIOS && !XAMARINMAC && !XAMARINTVOS && !MONOANDROID
    /// <summary>
    /// Set validation logic from DataAnnotations attributes.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="self">Target ReactiveProperty.</param>
    /// <param name="selfSelector">The self selector.</param>
    /// <returns>
    /// Self.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// selfSelector
    /// or
    /// self.
    /// </exception>
    /// <remarks>
    /// This method uses DataAnnotations validation which requires reflection and is not compatible with AOT compilation.
    /// For AOT scenarios, use manual validation instead.
    /// </remarks>
    [RequiresUnreferencedCode("DataAnnotations validation uses reflection to discover attributes and is not trim-safe. Use manual validation for AOT scenarios.")]
    public static ReactiveProperty<T> AddValidation<T>(this ReactiveProperty<T> self, Expression<Func<ReactiveProperty<T>?>> selfSelector)
    {
        ArgumentExceptionHelper.ThrowIfNull(selfSelector);
        ArgumentExceptionHelper.ThrowIfNull(self);

        var memberExpression = (MemberExpression)selfSelector.Body;
        var propertyInfo = (PropertyInfo)memberExpression.Member;
        var display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        var attrs = propertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();

        // Use the AOT-compatible constructor that doesn't require reflection for type discovery
        var context = new ValidationContext(self, serviceProvider: null, items: null)
        {
            DisplayName = display?.GetName() ?? propertyInfo.Name,
            MemberName = nameof(ReactiveProperty<T>.Value),
        };

        if (attrs.Length != 0)
        {
            self.AddValidationError(x =>
            {
                var validationResults = new List<ValidationResult>();
                if (System.ComponentModel.DataAnnotations.Validator.TryValidateValue(x!, context, validationResults, attrs))
                {
                    return null;
                }

                return validationResults[0].ErrorMessage;
            });
        }

        return self;
    }
#endif

    /// <summary>
    /// Create an IObservable instance to observe validation error messages of ReactiveProperty.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="self">Target ReactiveProperty.</param>
    /// <returns>A IObservable of string.</returns>
    public static IObservable<string?> ObserveValidationErrors<T>(this ReactiveProperty<T> self)
    {
        ArgumentExceptionHelper.ThrowIfNull(self);

        return self.ObserveErrorChanged
            .Select(static x => x?.OfType<string>()?.FirstOrDefault());
    }
}
