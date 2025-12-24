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

        Console.WriteLine($"[MauiLifecycleHelpers] TriggerAppearing called on {page.GetType().Name}");

        // First, check if there are any event handlers registered
        CheckEventHandlers(page, "Appearing");

        // SendAppearing() raises the event, while OnAppearing() is just a virtual method for derived classes
        // Try SendAppearing first, then OnAppearing as fallback
        var methodNames = new[] { "SendAppearing", "OnAppearing", "RaiseAppearing" };

        foreach (var methodName in methodNames)
        {
            var method = FindMethod(page.GetType(), methodName);
            if (method is not null)
            {
                Console.WriteLine($"[MauiLifecycleHelpers] Found and invoking {methodName}");
                InvokeMethod(page, method);

                // Check again after invoking
                CheckEventHandlers(page, "Appearing");
                return;
            }
        }

        // If no method found, try to manually invoke event handlers
        Console.WriteLine("[MauiLifecycleHelpers] No lifecycle method found, trying manual event trigger");
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

    private static void TriggerEventHandlers(object target, string eventName)
    {
        Console.WriteLine($"[MauiLifecycleHelpers] Attempting to trigger event handlers for {eventName} on {target.GetType().Name}");

        // Try to find the event backing field using different naming patterns
        var fieldPatterns = new[] { eventName, $"_{eventName}", $"{eventName}Event", $"_{char.ToLower(eventName[0])}{eventName.Substring(1)}" };
        FieldInfo? eventField = null;

        foreach (var pattern in fieldPatterns)
        {
            eventField = target.GetType().GetField(pattern, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (eventField is not null)
            {
                Console.WriteLine($"[MauiLifecycleHelpers] Found event field: {pattern}");
                break;
            }
        }

        if (eventField?.GetValue(target) is MulticastDelegate eventDelegate)
        {
            Console.WriteLine($"[MauiLifecycleHelpers] Found {eventDelegate.GetInvocationList().Length} event handlers for {eventName}");
            foreach (var handler in eventDelegate.GetInvocationList())
            {
                Console.WriteLine($"[MauiLifecycleHelpers] Invoking handler: {handler.Method.Name}");
                handler.Method.Invoke(handler.Target, [target, EventArgs.Empty]);
            }
        }
        else
        {
            Console.WriteLine($"[MauiLifecycleHelpers] No event field or handlers found for {eventName}. EventField: {eventField?.Name ?? "null"}, Value: {eventField?.GetValue(target)?.GetType().Name ?? "null"}");

            // Try to raise the event by calling the add accessor to see if subscriptions work
            var eventInfo = target.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo is not null)
            {
                Console.WriteLine($"[MauiLifecycleHelpers] Event {eventName} exists as a public event");
            }
        }
    }

    private static void CheckEventHandlers(object target, string eventName)
    {
        var eventInfo = target.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public);
        if (eventInfo is null)
        {
            Console.WriteLine($"[MauiLifecycleHelpers] Event {eventName} not found");
            return;
        }

        // Try to find the backing field
        var fieldPatterns = new[] { eventName, $"_{eventName}", $"{eventName}Event", $"_{char.ToLower(eventName[0])}{eventName.Substring(1)}" };
        foreach (var pattern in fieldPatterns)
        {
            var field = target.GetType().GetField(pattern, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field is not null)
            {
                var value = field.GetValue(target);
                if (value is MulticastDelegate del)
                {
                    Console.WriteLine($"[MauiLifecycleHelpers] {eventName} has {del.GetInvocationList().Length} handler(s)");
                    return;
                }

                Console.WriteLine($"[MauiLifecycleHelpers] {eventName} field found but has no handlers (value: {value?.GetType().Name ?? "null"})");
                return;
            }
        }

        Console.WriteLine($"[MauiLifecycleHelpers] {eventName} backing field not found - event may use different storage mechanism");
    }
}
