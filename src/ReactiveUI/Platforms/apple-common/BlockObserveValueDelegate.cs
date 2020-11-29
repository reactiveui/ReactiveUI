// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using Splat;

namespace ReactiveUI
{
    internal class BlockObserveValueDelegate : NSObject
    {
        private Action<string, NSObject, NSDictionary> _block;

        public BlockObserveValueDelegate(Action<string, NSObject, NSDictionary> block) => _block = block;

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context) => _block(keyPath, ofObject, change);
    }
}
