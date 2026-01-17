// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UIKitObservableForProperty provides toolkit-specific observable factories used by ReactiveUI
/// to generate change notifications for UIKit controls in <c>WhenAny*</c> and related operators.
/// </summary>
/// <remarks>
/// This implementation registers observable factories for common UIKit properties that change via
/// control events or notifications.
///
/// For event-based notifications, this implementation uses explicit add/remove handler overloads
/// (non-reflection) provided by <see cref="ObservableForPropertyBase"/> to improve performance and
/// trimming/AOT compatibility.
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
            static (sender, expr) => ObservableFromUIControlEvent(sender, expr, UIControlEvent.ValueChanged));

        // UITextField "Text" changes via notification.
        Register(
            typeof(UITextField),
            "Text",
            affinity: 30,
            static (sender, expr) => ObservableFromNotification(sender, expr, UITextField.TextFieldTextDidChangeNotification));

        // UITextView "Text" changes via notification.
        Register(
            typeof(UITextView),
            "Text",
            affinity: 30,
            static (sender, expr) => ObservableFromNotification(sender, expr, UITextView.TextDidChangeNotification));

        // UIDatePicker "Date" changes via ValueChanged.
        Register(
            typeof(UIDatePicker),
            "Date",
            affinity: 30,
            static (sender, expr) => ObservableFromUIControlEvent(sender, expr, UIControlEvent.ValueChanged));

        // UISegmentedControl "SelectedSegment" changes via ValueChanged.
        Register(
            typeof(UISegmentedControl),
            "SelectedSegment",
            affinity: 30,
            static (sender, expr) => ObservableFromUIControlEvent(sender, expr, UIControlEvent.ValueChanged));

        // UISwitch "On" changes via ValueChanged.
        Register(
            typeof(UISwitch),
            "On",
            affinity: 30,
            static (sender, expr) => ObservableFromUIControlEvent(sender, expr, UIControlEvent.ValueChanged));

        // Warning: This event-based approach may change the control's behavior depending on external delegate usage.
        // Use explicit add/remove to avoid reflection and trimming hazards.
        Register(
            typeof(UITabBar),
            "SelectedItem",
            affinity: 30,
            (sender, expr) =>
            {
                var tabBar = (UITabBar)sender;
                return ObservableFromEvent<UITabBar, UITabBarItemEventArgs>(
                    tabBar,
                    expr,
                    addHandler: h => tabBar.ItemSelected += h,
                    removeHandler: h => tabBar.ItemSelected -= h);
            });

        // Warning: This event-based approach may change the control's behavior depending on external delegate usage.
        // Use explicit add/remove to avoid reflection and trimming hazards.
        Register(
            typeof(UISearchBar),
            "Text",
            affinity: 30,
            (sender, expr) =>
            {
                var searchBar = (UISearchBar)sender;
                return ObservableFromEvent<UISearchBar, UISearchBarTextChangedEventArgs>(
                    searchBar,
                    expr,
                    addHandler: h => searchBar.TextChanged += h,
                    removeHandler: h => searchBar.TextChanged -= h);
            });
    }

    /// <summary>
    /// Gets the shared <see cref="UIKitObservableForProperty"/> instance.
    /// </summary>
    /// <remarks>
    /// The instance is created lazily. Consumers typically register it with the service locator once
    /// during application initialization.
    /// </remarks>
    public static Lazy<UIKitObservableForProperty> Instance { get; } = new();
}
