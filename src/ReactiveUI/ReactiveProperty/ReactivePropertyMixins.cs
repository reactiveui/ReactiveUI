﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

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
    /// <exception cref="System.ArgumentNullException">
    /// selfSelector
    /// or
    /// self.
    /// </exception>
    public static ReactiveProperty<T> AddValidation<T>(this ReactiveProperty<T> self, Expression<Func<ReactiveProperty<T>?>> selfSelector)
    {
        selfSelector.ArgumentNullExceptionThrowIfNull(nameof(selfSelector));
        self.ArgumentNullExceptionThrowIfNull(nameof(self));

        var memberExpression = (MemberExpression)selfSelector.Body;
        var propertyInfo = (PropertyInfo)memberExpression.Member;
        var display = propertyInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        var attrs = propertyInfo.GetCustomAttributes<System.ComponentModel.DataAnnotations.ValidationAttribute>().ToArray();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(self)
        {
            DisplayName = display?.GetName() ?? propertyInfo.Name,
            MemberName = nameof(ReactiveProperty<T>.Value),
        };

        if (attrs.Length != 0)
        {
            self.AddValidationError(x =>
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
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
        self.ArgumentNullExceptionThrowIfNull(nameof(self));

        return self.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault());
    }
}
