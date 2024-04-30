// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;
#pragma warning disable SA1402, SA1649, CA1040
/// <summary>
/// An interface for building.
/// </summary>
public interface IBuilder
{
}

/// <summary>
/// Default methods for the <see cref="IBuilder"/> abstraction.
/// </summary>
public static class IBuilderExtensions
{
    /// <summary>
    /// Adds the specified field to the builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder.</returns>
    public static TBuilder With<TBuilder, TField>(this TBuilder builder, out TField field, TField value)
        where TBuilder : IBuilder
    {
        field = value;
        return builder;
    }

    /// <summary>
    /// Adds the specified list of fields to the builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="field">The field.</param>
    /// <param name="values">The values.</param>
    /// <returns>The builder.</returns>
    public static TBuilder With<TBuilder, TField>(
        this TBuilder builder,
        ref List<TField>? field,
        IEnumerable<TField> values)
        where TBuilder : IBuilder
    {
        if (field is null)
        {
            throw new System.ArgumentNullException(nameof(field));
        }

        field.AddRange(values);

        return builder;
    }

    /// <summary>
    /// Adds the specified field to the builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder.</returns>
    public static TBuilder With<TBuilder, TField>(this TBuilder builder, ref List<TField>? field, TField value)
        where TBuilder : IBuilder
    {
        if (field is null)
        {
            throw new System.ArgumentNullException(nameof(field));
        }

        field.Add(value);
        return builder;
    }

    /// <summary>
    /// Adds the specified key value pair to the provided dictionary.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="keyValuePair">The key value pair.</param>
    /// <returns>The builder.</returns>
    public static TBuilder With<TBuilder, TKey, TField>(
        this TBuilder builder,
        ref Dictionary<TKey, TField> dictionary,
        KeyValuePair<TKey, TField> keyValuePair)
        where TBuilder : IBuilder
        where TKey : notnull
    {
        if (dictionary is null)
        {
            throw new System.ArgumentNullException(nameof(dictionary));
        }

        dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        return builder;
    }

    /// <summary>
    /// Adds the specified key and value to the provided dictionary.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder.</returns>
    public static TBuilder With<TBuilder, TKey, TField>(
        this TBuilder builder,
        ref Dictionary<TKey, TField> dictionary,
        TKey key,
        TField value)
        where TBuilder : IBuilder
        where TKey : notnull
    {
        if (dictionary is null)
        {
            throw new System.ArgumentNullException(nameof(dictionary));
        }

        dictionary.Add(key, value);
        return builder;
    }

    /// <summary>
    /// Adds the specified dictionary to the provided dictionary.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="builder">This builder.</param>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="keyValuePair">The key value pair.</param>
    /// <returns> The builder.</returns>
    public static TBuilder With<TBuilder, TKey, TField>(
        this TBuilder builder,
        ref Dictionary<TKey, TField> dictionary,
        IDictionary<TKey, TField> keyValuePair)
        where TKey : notnull
    {
        if (dictionary is null)
        {
            throw new System.ArgumentNullException(nameof(dictionary));
        }

        dictionary = (Dictionary<TKey, TField>)keyValuePair;
        return builder;
    }
}
#pragma warning restore SA1402, SA1649, CA1040