// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using AppKit;

namespace ReactiveUI
{
    [Preserve]
    public class AppKitObservableForProperty : ObservableForPropertyBase
    {
        public static Lazy<AppKitObservableForProperty> Instance = new Lazy<AppKitObservableForProperty>();

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
    }
}
