// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace IntegrationTests.Shared.Tests
{
    /// <summary>
    /// Extension methods that assist with builder type operations.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Sets a field to the specified value.
        /// </summary>
        /// <typeparam name="TBuilder">The type of builder.</typeparam>
        /// <typeparam name="TField">The type of field.</typeparam>
        /// <param name="this">The builder instance to use.</param>
        /// <param name="field">The reference to the field we are setting.</param>
        /// <param name="value">The new value of the field.</param>
        /// <returns>The builder instance.</returns>
        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref TField field, TField value)
            where TBuilder : IBuilder
        {
            field = value;
            return @this;
        }

        /// <summary>
        /// Sets a field to the specified enumerable value.
        /// It will add the values to the specified list, and won't override existing values.
        /// </summary>
        /// <typeparam name="TBuilder">The type of builder.</typeparam>
        /// <typeparam name="TField">The type of field.</typeparam>
        /// <param name="this">The builder instance to use.</param>
        /// <param name="field">The reference to the list field we are setting.</param>
        /// <param name="values">The new values of the field.</param>
        /// <returns>The builder instance.</returns>
        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref List<TField> field, IEnumerable<TField> values)
            where TBuilder : IBuilder
        {
            if (values == null)
            {
                field = null;
            }
            else
            {
                field.AddRange(values);
            }

            return @this;
        }

        /// <summary>
        /// Sets a list field to the specified value.
        /// It will add the value to the specified list, and won't override existing values.
        /// </summary>
        /// <typeparam name="TBuilder">The type of builder.</typeparam>
        /// <typeparam name="TField">The type of field.</typeparam>
        /// <param name="this">The builder instance to use.</param>
        /// <param name="field">The reference to the list field we are setting.</param>
        /// <param name="value">The new value to add to the list field.</param>
        /// <returns>The builder instance.</returns>
        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref List<TField> field, TField value)
            where TBuilder : IBuilder
        {
            field.Add(value);
            return @this;
        }
    }
}
