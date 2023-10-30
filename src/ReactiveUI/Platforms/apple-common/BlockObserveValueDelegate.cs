// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

namespace ReactiveUI;

internal class BlockObserveValueDelegate(Action<string, NSObject, NSDictionary> block) : NSObject
{
    public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context) => block(keyPath, ofObject, change);
}
