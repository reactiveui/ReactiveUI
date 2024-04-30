// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UIKitObservableForProperty is an object that knows how to
/// create notifications for a given type of object. Implement this if you
/// are porting RxUI to a new UI toolkit, or generally want to enable WhenAny
/// for another type of object that can be observed in a unique way.
/// </summary>
[Preserve]
public class UIKitObservableForProperty : ObservableForPropertyBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitObservableForProperty"/> class.
    /// </summary>
    public UIKitObservableForProperty()
    {
        Register(typeof(UIControl), "Value", 20, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UITextField), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextField.TextFieldTextDidChangeNotification));
        Register(typeof(UITextView), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextView.TextDidChangeNotification));
        Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));

        // Warning: This will stomp the Control's delegate
        Register(typeof(UITabBar), "SelectedItem", 30, (s, p) => ObservableFromEvent(s, p, "ItemSelected"));

        // Warning: This will stomp the Control's delegate
        Register(typeof(UISearchBar), "Text", 30, (s, p) => ObservableFromEvent(s, p, "TextChanged"));
    }

    /// <summary>
    /// Gets the UI Kit ObservableForProperty instance.
    /// </summary>
    public static Lazy<UIKitObservableForProperty> Instance { get; } = new();
}
