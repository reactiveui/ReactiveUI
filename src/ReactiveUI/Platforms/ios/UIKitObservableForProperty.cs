// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("UIKitObservableForProperty uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("UIKitObservableForProperty uses methods that may require unreferenced code")]
#endif
    public UIKitObservableForProperty()
    {
        Register(typeof(UIControl), "Value", 20, static (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UITextField), "Text", 30, static (s, p) => ObservableFromNotification(s, p, UITextField.TextFieldTextDidChangeNotification));
        Register(typeof(UITextView), "Text", 30, static (s, p) => ObservableFromNotification(s, p, UITextView.TextDidChangeNotification));
        Register(typeof(UIDatePicker), "Date", 30, static (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UISegmentedControl), "SelectedSegment", 30, static (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UISwitch), "On", 30, static (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
        Register(typeof(UISegmentedControl), "SelectedSegment", 30, static (s, p) => ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));

        // Warning: This will stomp the Control's delegate
        Register(typeof(UITabBar), "SelectedItem", 30, static (s, p) => ObservableFromEvent(s, p, "ItemSelected"));

        // Warning: This will stomp the Control's delegate
        Register(typeof(UISearchBar), "Text", 30, static (s, p) => ObservableFromEvent(s, p, "TextChanged"));
    }

    /// <summary>
    /// Gets the UI Kit ObservableForProperty instance.
    /// </summary>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Deliberate>")]
#endif
    public static Lazy<UIKitObservableForProperty> Instance { get; } = new();
}
