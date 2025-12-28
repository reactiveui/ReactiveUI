// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Legacy;

[ExcludeFromCodeCoverage]
internal sealed class RefcountDisposeWrapper(IDisposable inner)
{
    private IDisposable? _inner = inner;
    private int _refCount = 1;

    public void AddRef() => Interlocked.Increment(ref _refCount);

    public void Release()
    {
        if (Interlocked.Decrement(ref _refCount) == 0)
        {
            var inner = Interlocked.Exchange(ref _inner, null);
            inner?.Dispose();
        }
    }
}
