// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using AppKit;

namespace ReactiveUI
{
    /// <summary>
    /// AppKitObservableForProperty is an object that knows how to
    /// create notifications for a given type of object. Implement this if you
    /// are porting RxUI to a new UI toolkit, or generally want to enable WhenAny
    /// for another type of object that can be observed in a unique way.
    /// </summary>
    [Preserve]
    public class AppKitObservableForProperty : ObservableForPropertyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppKitObservableForProperty"/> class.
        /// </summary>
        public AppKitObservableForProperty()
        {
            Register(typeof(NSControl), "AlphaValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "DoubleValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "FloatValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "IntValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "NintValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "ObjectValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "StringValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
            Register(typeof(NSControl), "AttributedStringValue", 20, (s, p) => ObservableFromNotification(s, p, NSControl.TextDidChangeNotification));
        }

        /// <summary>
        /// Gets the App Kit ObservableForProperty instance.
        /// </summary>
        public static Lazy<AppKitObservableForProperty> Instance { get; } = new();
    }
}
