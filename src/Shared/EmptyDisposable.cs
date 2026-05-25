// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A no-op <see cref="IDisposable"/> singleton used in place of <c>Disposable.Empty</c>.
/// </summary>
internal sealed class EmptyDisposable : IDisposable
{
    /// <summary>
    /// The shared singleton instance.
    /// </summary>
    public static readonly EmptyDisposable Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyDisposable"/> class.
    /// </summary>
    private EmptyDisposable()
    {
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
