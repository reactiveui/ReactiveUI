// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>An <see cref="NSObject"/> delegate that forwards KVO observation callbacks to a caller-supplied block.</summary>
/// <param name="block">The callback invoked with the key path, observed object, and change dictionary on each KVO notification.</param>
internal class BlockObserveValueDelegate(Action<string, NSObject, NSDictionary> block) : NSObject
{
    /// <inheritdoc/>
    /// <param name="keyPath">The key path of the observed property that changed.</param>
    /// <param name="ofObject">The source object on which the change occurred.</param>
    /// <param name="change">The change dictionary describing the observed change.</param>
    /// <param name="context">The opaque context pointer supplied when the observer was registered.</param>
    public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context) => block(keyPath, ofObject, change);
}
