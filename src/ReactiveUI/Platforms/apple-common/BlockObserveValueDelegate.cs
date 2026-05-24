// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

namespace ReactiveUI;

/// <summary>
/// An <see cref="NSObject"/> delegate that forwards KVO observation callbacks to a caller-supplied block.
/// </summary>
internal class BlockObserveValueDelegate(Action<string, NSObject, NSDictionary> block) : NSObject
{
    /// <inheritdoc/>
    public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context) => block(keyPath, ofObject, change);
}
