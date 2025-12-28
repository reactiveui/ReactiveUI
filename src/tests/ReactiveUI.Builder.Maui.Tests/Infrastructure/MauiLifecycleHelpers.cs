// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using Microsoft.Maui.Controls;

namespace ReactiveUI.Builder.Maui.Tests.Infrastructure;

/// <summary>
/// Helper methods for triggering MAUI lifecycle events in tests.
/// </summary>
/// <remarks>
/// This class encapsulates reflection-based logic needed to trigger lifecycle events
/// that are not publicly accessible in the MAUI framework. This abstraction keeps
/// reflection isolated to one place rather than scattered throughout test code.
/// </remarks>
internal static class MauiLifecycleHelpers
{
    /// <summary>
    /// Triggers the Loaded lifecycle event on a view.
    /// </summary>
    /// <param name="view">The view to trigger the loaded event on.</param>
    /// <remarks>
    /// This method uses reflection to invoke internal MAUI methods that raise the Loaded event.
    /// In MAUI 9+, these methods are not publicly accessible, so reflection is required for test scenarios.
    /// </remarks>
    public static void TriggerLoaded(VisualElement view)
    {
        ArgumentNullException.ThrowIfNull(view);

        // Try to find and invoke methods that trigger the Loaded event
        // MAUI may have different internal method names across versions
        var methodNames = new[] { "SendLoaded", "OnLoaded", "RaiseLoaded" };

        foreach (var methodName in methodNames)
        {
            var method = FindMethod(view.GetType(), methodName);
            if (method is not null)
            {
                InvokeMethod(view, method);
                return;
            }
        }

        // If no method found, try to manually invoke event handlers
        TriggerEventHandlers(view, "Loaded");
    }

    /// <summary>
    /// Triggers the Unloaded lifecycle event on a view.
    /// </summary>
    /// <param name="view">The view to trigger the unloaded event on.</param>
    /// <remarks>
    /// This method uses reflection to invoke internal MAUI methods that raise the Unloaded event.
    /// In MAUI 9+, these methods are not publicly accessible, so reflection is required for test scenarios.
    /// </remarks>
    public static void TriggerUnloaded(VisualElement view)
    {
        ArgumentNullException.ThrowIfNull(view);

        // Try to find and invoke methods that trigger the Unloaded event
        var methodNames = new[] { "SendUnloaded", "OnUnloaded", "RaiseUnloaded" };

        foreach (var methodName in methodNames)
        {
            var method = FindMethod(view.GetType(), methodName);
            if (method is not null)
            {
                InvokeMethod(view, method);
                return;
            }
        }

        // If no method found, try to manually invoke event handlers
        TriggerEventHandlers(view, "Unloaded");
    }

    /// <summary>
    /// Triggers the Appearing lifecycle event on a page.
    /// </summary>
    /// <param name="page">The page to trigger the appearing event on.</param>
    /// <remarks>
    /// This method uses reflection to invoke internal MAUI methods that raise the Appearing event.
    /// In MAUI 9+, these methods are not publicly accessible, so reflection is required for test scenarios.
    /// </remarks>
    public static void TriggerAppearing(Page page)
    {
        ArgumentNullException.ThrowIfNull(page);

        // SendAppearing() raises the event, while OnAppearing() is just a virtual method for derived classes
        // Try SendAppearing first, then OnAppearing as fallback
        var methodNames = new[] { "SendAppearing", "OnAppearing", "RaiseAppearing" };

        foreach (var methodName in methodNames)
        {
            var method = FindMethod(page.GetType(), methodName);
            if (method is not null)
            {
                InvokeMethod(page, method);
                return;
            }
        }

        // If no method found, try to manually invoke event handlers
        TriggerEventHandlers(page, "Appearing");
    }

    /// <summary>
    /// Triggers the Disappearing lifecycle event on a page.
    /// </summary>
    /// <param name="page">The page to trigger the disappearing event on.</param>
    /// <remarks>
    /// This method uses reflection to invoke internal MAUI methods that raise the Disappearing event.
    /// In MAUI 9+, these methods are not publicly accessible, so reflection is required for test scenarios.
    /// </remarks>
    public static void TriggerDisappearing(Page page)
    {
        ArgumentNullException.ThrowIfNull(page);

        // OnDisappearing() is the protected virtual method that's meant to be called to trigger the lifecycle
        // Try OnDisappearing first, then fallback to SendDisappearing
        var methodNames = new[] { "OnDisappearing", "SendDisappearing", "RaiseDisappearing" };

        foreach (var methodName in methodNames)
        {
            var method = FindMethod(page.GetType(), methodName);
            if (method is not null)
            {
                InvokeMethod(page, method);
                return;
            }
        }

        // If no method found, try to manually invoke event handlers
        TriggerEventHandlers(page, "Disappearing");
    }

    /// <summary>
    /// Searches for a method with the specified name in the type hierarchy.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <param name="methodName">The name of the method to find.</param>
    /// <returns>The found method, or <see langword="null"/> if no matching method exists.</returns>
    /// <remarks>
    /// Searches the entire inheritance chain and prefers parameterless methods, then methods
    /// accepting a single EventArgs parameter, then any other matching method.
    /// </remarks>
    private static MethodInfo? FindMethod(Type type, string methodName)
    {
        var currentType = type;
        while (currentType is not null)
        {
            var methods = currentType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.Name == methodName)
                .ToList();

            if (methods.Count > 0)
            {
                // Prefer parameterless methods
                return methods.FirstOrDefault(m => m.GetParameters().Length == 0) ??
                       methods.FirstOrDefault(m => m.GetParameters().Length == 1 &&
                                                  typeof(EventArgs).IsAssignableFrom(m.GetParameters()[0].ParameterType)) ??
                       methods.First();
            }

            currentType = currentType.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Invokes a lifecycle method with appropriate parameters based on its signature.
    /// </summary>
    /// <param name="target">The object on which to invoke the method.</param>
    /// <param name="method">The method to invoke.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the method has an unsupported signature.
    /// </exception>
    /// <remarks>
    /// Supports parameterless methods and methods that accept a single EventArgs parameter.
    /// </remarks>
    private static void InvokeMethod(object target, MethodInfo method)
    {
        var parameters = method.GetParameters();
        var args = parameters.Length switch
        {
            0 => Array.Empty<object>(),
            1 when typeof(EventArgs).IsAssignableFrom(parameters[0].ParameterType) => [EventArgs.Empty],
            _ => throw new InvalidOperationException($"Unsupported method signature for {method.Name}")
        };

        method.Invoke(target, args);
    }

    /// <summary>
    /// Manually invokes all event handlers subscribed to an event by accessing its backing field.
    /// </summary>
    /// <param name="target">The object whose event handlers should be invoked.</param>
    /// <param name="eventName">The name of the event to trigger.</param>
    /// <remarks>
    /// This is a fallback mechanism used when the lifecycle trigger methods cannot be found.
    /// It searches for the event backing field using common naming patterns and invokes
    /// all subscribed handlers directly.
    /// </remarks>
    private static void TriggerEventHandlers(object target, string eventName)
    {
        // Try to find the event backing field using different naming patterns
        var fieldPatterns = new[] { eventName, $"_{eventName}", $"{eventName}Event", $"_{char.ToLower(eventName[0])}{eventName.Substring(1)}" };
        FieldInfo? eventField = null;

        foreach (var pattern in fieldPatterns)
        {
            eventField = target.GetType().GetField(pattern, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (eventField is not null)
            {
                break;
            }
        }

        if (eventField?.GetValue(target) is MulticastDelegate eventDelegate)
        {
            foreach (var handler in eventDelegate.GetInvocationList())
            {
                handler.Method.Invoke(handler.Target, [target, EventArgs.Empty]);
            }
        }
    }
}
