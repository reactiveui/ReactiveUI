// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>A <see cref="ReactiveProperty{T}"/> that prints its field name when it is disposed.</summary>
/// <typeparam name="T">The type of the value the property holds.</typeparam>
[System.Diagnostics.DebuggerDisplay("{_fieldName}, IsDisposed = {IsDisposed}")]
public sealed class LoggingReactiveProperty<T> : ReactiveProperty<T>
{
    /// <summary>The form field this property backs, printed when the property is disposed.</summary>
    private readonly string _fieldName;

    /// <summary>Initializes a new instance of the <see cref="LoggingReactiveProperty{T}"/> class.</summary>
    /// <param name="fieldName">The form field this property backs.</param>
    /// <param name="initialValue">The initial value.</param>
    public LoggingReactiveProperty(string fieldName, T? initialValue)
        : base(initialValue) => _fieldName = fieldName;

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Console.WriteLine($"Disposed {_fieldName}");
        }

        base.Dispose(disposing);
    }
}
