// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Testing;

/// <summary>Default methods for the <see cref="IBuilder"/> abstraction.</summary>
[SuppressMessage("Design", "CA1045:Do not pass types by reference", Justification = "The fluent builder mutates caller fields/collections in place by design.")]
public static partial class IBuilderExtensions
{
    /// <summary>Provides fluent configuration extension members for builders.</summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <param name="builder">This builder.</param>
    extension<TBuilder>(TBuilder builder)
        where TBuilder : IBuilder
    {
        /// <summary>Adds the specified field to the builder.</summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The builder.</returns>
        public TBuilder With<TField>(out TField field, TField value)
        {
            field = value;
            return builder;
        }

        /// <summary>Adds the specified list of fields to the builder.</summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns>The builder.</returns>
        public TBuilder With<TField>(
            ref List<TField>? field,
            IEnumerable<TField> values)
        {
            ArgumentExceptionHelper.ThrowIfNull(field);

            field.AddRange(values);

            return builder;
        }

        /// <summary>Adds the specified field to the builder.</summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The builder.</returns>
        public TBuilder With<TField>(ref List<TField>? field, TField value)
        {
            ArgumentExceptionHelper.ThrowIfNull(field);

            field.Add(value);
            return builder;
        }

        /// <summary>Adds the specified key value pair to the provided dictionary.</summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>The builder.</returns>
        public TBuilder With<TKey, TField>(
            ref Dictionary<TKey, TField> dictionary,
            KeyValuePair<TKey, TField> keyValuePair)
            where TKey : notnull
        {
            ArgumentExceptionHelper.ThrowIfNull(dictionary);

            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            return builder;
        }

        /// <summary>Adds the specified key and value to the provided dictionary.</summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The builder.</returns>
        public TBuilder With<TKey, TField>(
            ref Dictionary<TKey, TField> dictionary,
            TKey key,
            TField value)
            where TKey : notnull
        {
            ArgumentExceptionHelper.ThrowIfNull(dictionary);

            dictionary.Add(key, value);
            return builder;
        }
    }
}

/// <summary>Provides fluent dictionary-replacement extension members for builders whose receiver type is unconstrained.</summary>
[SuppressMessage("Design", "CA1045:Do not pass types by reference", Justification = "The fluent builder mutates caller fields/collections in place by design.")]
public static partial class IBuilderExtensions
{
    /// <summary>Provides fluent dictionary-replacement extension members for builders.</summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <param name="builder">This builder.</param>
    extension<TBuilder>(TBuilder builder)
    {
        /// <summary>Adds the specified dictionary to the provided dictionary.</summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns> The builder.</returns>
        public TBuilder With<TKey, TField>(
            ref Dictionary<TKey, TField> dictionary,
            IDictionary<TKey, TField> keyValuePair)
            where TKey : notnull
        {
            ArgumentExceptionHelper.ThrowIfNull(dictionary);

            dictionary = (Dictionary<TKey, TField>)keyValuePair;
            return builder;
        }
    }
}
