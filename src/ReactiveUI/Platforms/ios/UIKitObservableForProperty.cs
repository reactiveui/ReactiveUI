// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using UIKit;

namespace ReactiveUI
{
    [Preserve]
    public class UIKitObservableForProperty : ObservableForPropertyBase
    {
        public static Lazy<UIKitObservableForProperty> Instance = new Lazy<UIKitObservableForProperty>();

        public UIKitObservableForProperty()
        {
            Register(typeof(UIControl), "Value", 20, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UITextField), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextField.TextFieldTextDidChangeNotification));
            Register(typeof(UITextView), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextView.TextDidChangeNotification));
            Register(typeof(UIDatePicker), "Date", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISwitch), "On", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));

            // Warning: This will stomp the Control's delegate
            Register(typeof(UITabBar), "SelectedItem", 30, (s, p) => ObservableFromEvent(s, p, "ItemSelected"));

            // Warning: This will stomp the Control's delegate
            Register(typeof(UISearchBar), "Text", 30, (s, p) => ObservableFromEvent(s, p, "TextChanged"));
        }
    }
}

