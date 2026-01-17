// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// Provides UIKit-specific observable factories used by ReactiveUI to generate change notifications for
/// UIKit controls in <c>WhenAny*</c> and related operators.
/// </summary>
/// <remarks>
/// This implementation registers observable factories for common UIKit properties that change via control
/// events or notifications.
/// </remarks>
[Preserve]
public sealed class UIKitObservableForProperty : ObservableForPropertyBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitObservableForProperty"/> class.
    /// </summary>
    public UIKitObservableForProperty()
    {
        // UIControl "Value" changes via ValueChanged.
        Register(
            typeof(UIControl),
            "Value",
            affinity: 20,
            static (sender, expression) => ObservableFromUIControlEvent(sender, expression, UIControlEvent.ValueChanged));

        // UITextField "Text" changes via notification.
        Register(
            typeof(UITextField),
            "Text",
            affinity: 30,
            static (sender, expression) => ObservableFromNotification(sender, expression, UITextField.TextFieldTextDidChangeNotification));

        // UITextView "Text" changes via notification.
        Register(
            typeof(UITextView),
            "Text",
            affinity: 30,
            static (sender, expression) => ObservableFromNotification(sender, expression, UITextView.TextDidChangeNotification));

        // UISegmentedControl "SelectedSegment" changes via ValueChanged.
        Register(
            typeof(UISegmentedControl),
            "SelectedSegment",
            affinity: 30,
            static (sender, expression) => ObservableFromUIControlEvent(sender, expression, UIControlEvent.ValueChanged));

        // Warning: Event-based observation can impact control behavior depending on external delegate usage.
        // Prefer explicit add/remove handler overloads (non-reflection) to improve performance and trimming/AOT compatibility.
        Register(
            typeof(UITabBar),
            "SelectedItem",
            affinity: 30,
            static (sender, expression) =>
            {
                var tabBar = (UITabBar)sender;
                return ObservableFromEvent<UITabBar, UITabBarItemEventArgs>(
                    tabBar,
                    expression,
                    addHandler: h => tabBar.ItemSelected += h,
                    removeHandler: h => tabBar.ItemSelected -= h);
            });

        // Warning: Event-based observation can impact control behavior depending on external delegate usage.
        // Prefer explicit add/remove handler overloads (non-reflection) to improve performance and trimming/AOT compatibility.
        Register(
            typeof(UISearchBar),
            "Text",
            affinity: 30,
            static (sender, expression) =>
            {
                var searchBar = (UISearchBar)sender;
                return ObservableFromEvent<UISearchBar, UISearchBarTextChangedEventArgs>(
                    searchBar,
                    expression,
                    addHandler: h => searchBar.TextChanged += h,
                    removeHandler: h => searchBar.TextChanged -= h);
            });
    }

    /// <summary>
    /// Gets the shared <see cref="UIKitObservableForProperty"/> instance.
    /// </summary>
    /// <remarks>
    /// The instance is created lazily. Consumers typically register it with the service locator once during
    /// application initialization.
    /// </remarks>
    public static Lazy<UIKitObservableForProperty> Instance { get; } = new();
}
