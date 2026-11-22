// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !XAMARINIOS && !XAMARINMAC && !XAMARINTVOS && !MONOANDROID
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Provides reflection-based DataAnnotations validation extensions for <see cref="ReactiveProperty{T}"/>.</summary>
public static class UnsafeReactivePropertyMixins
{
    /// <summary>Provides DataAnnotations validation extension members for <see cref="ReactiveProperty{T}"/>.</summary>
    /// <typeparam name="T">The type of the value held by the reactive property.</typeparam>
    /// <param name="self">The reactive property the validation members operate on. Cannot be null.</param>
    extension<T>(ReactiveProperty<T> self)
    {
        /// <summary>
        /// Adds DataAnnotations-based validation to the specified reactive property using the validation attributes defined
        /// on the property.
        /// </summary>
        /// <param name="selfSelector">An expression that selects the reactive property to be validated. This is used to discover validation attributes
        /// and display metadata. Cannot be null.</param>
        /// <returns>The same reactive property instance with validation enabled based on the discovered validation attributes.</returns>
        /// <remarks>This method uses reflection to discover DataAnnotations attributes and is not compatible with
        /// trimming or AOT scenarios. For environments where reflection is restricted, use manual validation instead. The
        /// validation logic is based on the attributes applied to the property referenced by the selector
        /// expression.</remarks>
        [RequiresUnreferencedCode(
            "DataAnnotations validation uses reflection to discover attributes and is not trim-safe. Use manual validation for AOT scenarios.")]
        public ReactiveProperty<T> AddValidation(
            Expression<Func<ReactiveProperty<T>?>> selfSelector)
        {
            ArgumentExceptionHelper.ThrowIfNull(selfSelector);
            ArgumentExceptionHelper.ThrowIfNull(self);

            var propertyInfo = (PropertyInfo)((MemberExpression)selfSelector.Body).Member;
            var display = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            ValidationAttribute[] attrs = [.. propertyInfo.GetCustomAttributes<ValidationAttribute>()];

            ValidationContext context = new(self, null, null) { DisplayName = display?.GetName() ?? propertyInfo.Name, MemberName = nameof(ReactiveProperty<>.Value) };

            if (attrs.Length != 0)
            {
                _ = self.AddValidationError(x =>
                {
                    List<ValidationResult> validationResults = [];
                    return Validator.TryValidateValue(x!, context, validationResults, attrs) ? null : validationResults[0].ErrorMessage;
                });
            }

            return self;
        }
    }
}
#endif
