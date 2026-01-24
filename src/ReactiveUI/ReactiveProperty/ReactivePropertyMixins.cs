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
/// Provides extension methods for adding validation and observing validation errors on ReactiveProperty instances.
/// </summary>
/// <remarks>These mixins enable integration of DataAnnotations-based validation and error observation with
/// ReactiveProperty. Some methods may use reflection and are not compatible with ahead-of-time (AOT) compilation; refer
/// to individual method documentation for details.</remarks>
public static class ReactivePropertyMixins
{
#if !XAMARINIOS && !XAMARINMAC && !XAMARINTVOS && !MONOANDROID
    /// <summary>
    /// Adds DataAnnotations-based validation to the specified reactive property using the validation attributes defined
    /// on the property.
    /// </summary>
    /// <remarks>This method uses reflection to discover DataAnnotations attributes and is not compatible with
    /// trimming or AOT scenarios. For environments where reflection is restricted, use manual validation instead. The
    /// validation logic is based on the attributes applied to the property referenced by the selector
    /// expression.</remarks>
    /// <typeparam name="T">The type of the value held by the reactive property.</typeparam>
    /// <param name="self">The reactive property to which validation will be added. Cannot be null.</param>
    /// <param name="selfSelector">An expression that selects the reactive property to be validated. This is used to discover validation attributes
    /// and display metadata. Cannot be null.</param>
    /// <returns>The same reactive property instance with validation enabled based on the discovered validation attributes.</returns>
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
    /// Returns an observable sequence that emits the first validation error message, if any, from the specified
    /// reactive property whenever its error state changes.
    /// </summary>
    /// <typeparam name="T">The type of the value held by the reactive property.</typeparam>
    /// <param name="self">The reactive property to observe for validation errors. Cannot be null.</param>
    /// <returns>An observable sequence of strings representing the first validation error message, or null if there are no
    /// errors.</returns>
    public static IObservable<string?> ObserveValidationErrors<T>(this ReactiveProperty<T> self)
    {
        ArgumentExceptionHelper.ThrowIfNull(self);

        return self.ObserveErrorChanged
            .Select(static x => x?.OfType<string>()?.FirstOrDefault());
    }
}
