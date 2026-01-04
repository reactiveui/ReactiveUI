// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#nullable enable

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using Android.App;
using Android.Views;

using ReactiveUI.Helpers;

namespace ReactiveUI;

/// <summary>
/// Control fetcher helpers for Android that support wiring up properties to Android resource IDs by name.
/// </summary>
/// <remarks>
/// <para>
/// This API is intended for classic Android view wiring patterns (e.g., Activities/Fragments/Views).
/// It performs name-to-resource-id resolution using reflection over the generated Android Resource classes,
/// and caches lookups per assembly and per root view.
/// </para>
/// <para>
/// Trimming/AOT: resource discovery uses reflection over generated resource types and may require preserving
/// those members. See <see cref="GetControlIdByName(Assembly, string)"/> and related remarks.
/// </para>
/// </remarks>
public static partial class ControlFetcherMixin
{
    /// <summary>
    /// Cache mapping an assembly to a case-insensitive resource-name→id map.
    /// </summary>
    /// <remarks>
    /// This cache is populated on demand. The per-assembly map is immutable after creation to allow lock-free reads.
    /// </remarks>
    private static readonly ConcurrentDictionary<Assembly, IReadOnlyDictionary<string, int>> ControlIds = new();

    /// <summary>
    /// Cache mapping a root view object to a per-property cached <see cref="View"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This cache avoids repeated <c>FindViewById</c> calls for the same root and property name.
    /// </para>
    /// <para>
    /// Threading: Android UI access is typically single-threaded; however, this cache uses a concurrent dictionary
    /// to avoid race conditions if called from multiple threads (e.g., during tests or unusual scheduling).
    /// </para>
    /// </remarks>
    private static readonly ConditionalWeakTable<object, ConcurrentDictionary<string, View?>> ViewCache = new();

    /// <summary>
    /// Cache of wire-up property lists per runtime type and resolve strategy.
    /// </summary>
    /// <remarks>
    /// This avoids repeated reflection over properties when wiring up controls multiple times.
    /// </remarks>
    private static readonly ConcurrentDictionary<(Type Type, ResolveStrategy Strategy), PropertyInfo[]> WireUpMembersCache = new();

    /// <summary>
    /// Gets a control from an <see cref="Activity"/> using the calling member name as the default resource name.
    /// </summary>
    /// <param name="activity">The activity that hosts the view hierarchy.</param>
    /// <param name="propertyName">
    /// The property name to use as the resource identifier. Defaults to the calling member name.
    /// </param>
    /// <returns>The resolved view if found; otherwise <see langword="null"/>.</returns>
    [RequiresUnreferencedCode("Android resource discovery uses reflection over generated resource types that may be trimmed.")]
    [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
    public static View? GetControl(this Activity activity, [CallerMemberName] string? propertyName = null) =>
        GetCachedControl(
            propertyName,
            activity,
            static (root, name) =>
            {
                var act = (Activity)root;
                var id = GetControlIdByName(act.GetType().Assembly, name);
                return act.FindViewById(id);
            });

    /// <summary>
    /// Gets a control from an Android <see cref="View"/> using the calling member name as the default resource name.
    /// </summary>
    /// <param name="view">The root view.</param>
    /// <param name="assembly">The assembly containing the user-defined view and its resources.</param>
    /// <param name="propertyName">
    /// The property name to use as the resource identifier. Defaults to the calling member name.
    /// </param>
    /// <returns>The resolved view if found; otherwise <see langword="null"/>.</returns>
    [RequiresUnreferencedCode("Android resource discovery uses reflection over generated resource types that may be trimmed.")]
    [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
    public static View? GetControl(this View view, Assembly assembly, [CallerMemberName] string? propertyName = null) =>
        GetCachedControl(
            propertyName,
            view,
            static (root, name, state) =>
            {
                var v = (View)root;
                var id = GetControlIdByName(state, name);
                return v.FindViewById(id);
            },
            assembly);

    /// <summary>
    /// Wires view controls to properties on an <see cref="ILayoutViewHost"/>.
    /// </summary>
    /// <param name="layoutHost">The layout host that exposes a <see cref="ILayoutViewHost.View"/>.</param>
    /// <param name="resolveMembers">The resolve strategy for selecting properties to wire.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="layoutHost"/> is null.</exception>
    /// <exception cref="MissingFieldException">
    /// Thrown when a property cannot be wired to a view with a corresponding resource identifier.
    /// </exception>
    [RequiresUnreferencedCode("WireUpControls uses reflection to discover properties and attributes that may be trimmed.")]
    [RequiresDynamicCode("WireUpControls uses reflection that may require dynamic code generation.")]
    public static void WireUpControls(this ILayoutViewHost layoutHost, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
    {
        ArgumentExceptionHelper.ThrowIfNull(layoutHost);

        var hostType = layoutHost.GetType();
        var members = GetWireUpMembersCached(hostType, resolveMembers);

        for (var i = 0; i < members.Length; i++)
        {
            var member = members[i];

            try
            {
                var root = layoutHost.View;
                var resourceName = member.GetResourceName();

                var resolved = root is null
                    ? null
                    : root.GetControl(hostType.Assembly, resourceName);

                member.SetValue(layoutHost, resolved);
            }
            catch (Exception ex)
            {
                throw new MissingFieldException(
                    "Failed to wire up the Property " + member.Name +
                    " to a View in your layout with a corresponding identifier.",
                    ex);
            }
        }
    }

    /// <summary>
    /// Wires view controls to properties on an Android <see cref="View"/>.
    /// </summary>
    /// <param name="view">The view whose properties should be wired.</param>
    /// <param name="resolveMembers">The resolve strategy for selecting properties to wire.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/> is null.</exception>
    /// <exception cref="MissingFieldException">
    /// Thrown when a property cannot be wired to a view with a corresponding resource identifier.
    /// </exception>
    [RequiresUnreferencedCode("WireUpControls uses reflection to discover properties and attributes that may be trimmed.")]
    [RequiresDynamicCode("WireUpControls uses reflection that may require dynamic code generation.")]
    public static void WireUpControls(this View view, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        var viewType = view.GetType();
        var members = GetWireUpMembersCached(viewType, resolveMembers);
        var asm = viewType.Assembly;

        for (var i = 0; i < members.Length; i++)
        {
            var member = members[i];

            try
            {
                var resourceName = member.GetResourceName();
                var currentView = view.GetControl(asm, resourceName);
                member.SetValue(view, currentView);
            }
            catch (Exception ex)
            {
                throw new MissingFieldException(
                    "Failed to wire up the Property " + member.Name +
                    " to a View in your layout with a corresponding identifier.",
                    ex);
            }
        }
    }

    /// <summary>
    /// Wires view controls to properties on an Android <see cref="Fragment"/>.
    /// </summary>
    /// <param name="fragment">The fragment whose properties should be wired.</param>
    /// <param name="inflatedView">The inflated view returned from <c>OnCreateView</c>.</param>
    /// <param name="resolveMembers">The resolve strategy for selecting properties to wire.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="fragment"/> or <paramref name="inflatedView"/> is null.
    /// </exception>
    /// <exception cref="MissingFieldException">
    /// Thrown when a property cannot be wired to a view with a corresponding resource identifier.
    /// </exception>
    [RequiresUnreferencedCode("WireUpControls uses reflection to discover properties and attributes that may be trimmed.")]
    [RequiresDynamicCode("WireUpControls uses reflection that may require dynamic code generation.")]
    public static void WireUpControls(this Fragment fragment, View inflatedView, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
    {
        ArgumentExceptionHelper.ThrowIfNull(fragment);
        ArgumentExceptionHelper.ThrowIfNull(inflatedView);

        var fragmentType = fragment.GetType();
        var members = GetWireUpMembersCached(fragmentType, resolveMembers);
        var asm = fragmentType.Assembly;

        for (var i = 0; i < members.Length; i++)
        {
            var member = members[i];

            try
            {
                var resourceName = member.GetResourceName();
                var resolved = inflatedView.GetControl(asm, resourceName);
                member.SetValue(fragment, resolved);
            }
            catch (Exception ex)
            {
                throw new MissingFieldException(
                    "Failed to wire up the Property " + member.Name +
                    " to a View in your layout with a corresponding identifier.",
                    ex);
            }
        }
    }

    /// <summary>
    /// Wires view controls to properties on an <see cref="Activity"/>.
    /// </summary>
    /// <param name="activity">The activity whose properties should be wired.</param>
    /// <param name="resolveMembers">The resolve strategy for selecting properties to wire.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
    /// <exception cref="MissingFieldException">
    /// Thrown when a property cannot be wired to a view with a corresponding resource identifier.
    /// </exception>
    [RequiresUnreferencedCode("WireUpControls uses reflection to discover properties and attributes that may be trimmed.")]
    [RequiresDynamicCode("WireUpControls uses reflection that may require dynamic code generation.")]
    public static void WireUpControls(this Activity activity, ResolveStrategy resolveMembers = ResolveStrategy.Implicit)
    {
        ArgumentExceptionHelper.ThrowIfNull(activity);

        var activityType = activity.GetType();
        var members = GetWireUpMembersCached(activityType, resolveMembers);

        for (var i = 0; i < members.Length; i++)
        {
            var member = members[i];

            try
            {
                var resourceName = member.GetResourceName();
                var resolved = activity.GetControl(resourceName);
                member.SetValue(activity, resolved);
            }
            catch (Exception ex)
            {
                throw new MissingFieldException(
                    "Failed to wire up the Property " + member.Name +
                    " to a View in your layout with a corresponding identifier.",
                    ex);
            }
        }
    }

    /// <summary>
    /// Retrieves the set of properties on the specified object that are eligible for wire-up based on the provided
    /// resolution strategy.
    /// </summary>
    /// <remarks>This method uses reflection to discover properties, which may require dynamically generated
    /// code and may be affected by code trimming. Use caution when calling this method in environments where reflection
    /// or dynamic code generation is restricted.</remarks>
    /// <param name="this">The object whose properties are to be discovered for wire-up.</param>
    /// <param name="resolveStrategy">The strategy that determines which properties are considered for wire-up.</param>
    /// <returns>An enumerable collection of <see cref="PropertyInfo"/> objects representing the properties eligible for wire-up.
    /// The collection is empty if no matching properties are found.</returns>
    [RequiresUnreferencedCode("Property discovery uses reflection and may require members removed by trimming.")]
    [RequiresDynamicCode("Property discovery uses reflection that may require dynamic code generation.")]
    internal static PropertyInfo[] GetWireUpMembers(this object @this, ResolveStrategy resolveStrategy)
    {
        var type = @this.GetType();

        return GetWireUpMembers(type, resolveStrategy);
    }

    /// <summary>
    /// Returns the set of properties eligible for wiring for a given runtime type and strategy.
    /// </summary>
    /// <param name="type">The runtime type.</param>
    /// <param name="resolveStrategy">The property selection strategy.</param>
    /// <returns>An array of properties eligible for wiring.</returns>
    [RequiresUnreferencedCode("Property discovery uses reflection and may require members removed by trimming.")]
    [RequiresDynamicCode("Property discovery uses reflection that may require dynamic code generation.")]
    internal static PropertyInfo[] GetWireUpMembersCached(Type type, ResolveStrategy resolveStrategy) =>
        WireUpMembersCache.GetOrAdd((type, resolveStrategy), static key =>
        {
            var members = key.Type.GetRuntimeProperties();

            // Materialize once into a list then to array; no LINQ in per-wire loops.
            var list = new List<PropertyInfo>();

            foreach (var member in members)
            {
                if (!member.CanWrite)
                {
                    continue;
                }

                switch (key.Strategy)
                {
                    case ResolveStrategy.ExplicitOptIn:
                        if (member.GetCustomAttribute<WireUpResourceAttribute>(inherit: true) is not null)
                        {
                            list.Add(member);
                        }

                        break;

                    case ResolveStrategy.ExplicitOptOut:
                        if (typeof(View).IsAssignableFrom(member.PropertyType) &&
                            member.GetCustomAttribute<IgnoreResourceAttribute>(inherit: true) is null)
                        {
                            list.Add(member);
                        }

                        break;

                    default:
                        // Implicit: either a View-typed property or explicitly marked with WireUpResource.
                        if (member.PropertyType.IsSubclassOf(typeof(View)) ||
                            member.GetCustomAttribute<WireUpResourceAttribute>(inherit: true) is not null)
                        {
                            list.Add(member);
                        }

                        break;
                }
            }

            return list.ToArray();
        });

    /// <summary>
    /// Gets the resource name for the specified property based on optional overrides.
    /// </summary>
    /// <param name="member">The property being wired.</param>
    /// <returns>The resource name to use.</returns>
    [RequiresUnreferencedCode("Attribute lookup uses reflection and may require members removed by trimming.")]
    [RequiresDynamicCode("Attribute lookup uses reflection that may require dynamic code generation.")]
    internal static string GetResourceName(this PropertyInfo member)
    {
        var attr = member.GetCustomAttribute<WireUpResourceAttribute>();
        return attr?.ResourceNameOverride ?? member.Name;
    }

    /// <summary>
    /// Gets a cached control for a root view and property name, fetching it if absent.
    /// </summary>
    /// <param name="propertyName">The cache key, typically the property name.</param>
    /// <param name="rootView">The root view object used as cache scope.</param>
    /// <param name="fetchControlFromView">Factory used to fetch the view when not cached.</param>
    /// <returns>The cached view (possibly null).</returns>
    private static View? GetCachedControl(
        string? propertyName,
        object rootView,
        Func<object, string, View?> fetchControlFromView)
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(fetchControlFromView);

        var cache = ViewCache.GetOrCreateValue(rootView);

        if (cache.TryGetValue(propertyName, out var existing))
        {
            return existing;
        }

        var created = fetchControlFromView(rootView, propertyName);

        // ConcurrentDictionary indexer is safe; last write wins in a race.
        cache[propertyName] = created;
        return created;
    }

    /// <summary>
    /// Gets a cached control for a root view and property name, fetching it if absent, with an extra state value.
    /// </summary>
    /// <typeparam name="TState">The type of state passed to the fetch function.</typeparam>
    /// <param name="propertyName">The cache key, typically the property name.</param>
    /// <param name="rootView">The root view object used as cache scope.</param>
    /// <param name="fetchControlFromView">Factory used to fetch the view when not cached.</param>
    /// <param name="state">State passed to the fetch factory.</param>
    /// <returns>The cached view (possibly null).</returns>
    private static View? GetCachedControl<TState>(
        string? propertyName,
        object rootView,
        Func<object, string, TState, View?> fetchControlFromView,
        TState state)
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(fetchControlFromView);

        var cache = ViewCache.GetOrCreateValue(rootView);

        if (cache.TryGetValue(propertyName, out var existing))
        {
            return existing;
        }

        var created = fetchControlFromView(rootView, propertyName, state);
        cache[propertyName] = created;
        return created;
    }

    /// <summary>
    /// Resolves the Android resource ID for the given resource name within the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly whose generated resource types should be inspected.</param>
    /// <param name="name">The resource name.</param>
    /// <returns>The resolved integer resource ID.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="MissingFieldException">Thrown when the name cannot be resolved to an ID.</exception>
    [RequiresUnreferencedCode("Android resource discovery uses reflection over generated resource types that may be trimmed.")]
    [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
    private static int GetControlIdByName(Assembly assembly, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Resource name must not be null or whitespace.", nameof(name));
        }

        var ids = ControlIds.GetOrAdd(assembly, static asm => BuildIdMap(asm));

        if (ids.TryGetValue(name, out var id))
        {
            return id;
        }

        throw new MissingFieldException($"No Android resource id named '{name}' was found for assembly '{assembly.FullName}'.");
    }

    /// <summary>
    /// Builds an immutable mapping of resource name to integer ID for an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>A case-insensitive mapping of resource name to ID.</returns>
    [RequiresUnreferencedCode("Android resource discovery uses reflection over generated resource types that may be trimmed.")]
    [RequiresDynamicCode("Android resource discovery uses reflection that may require dynamic code generation.")]
    private static IReadOnlyDictionary<string, int> BuildIdMap(Assembly assembly)
    {
#if NET8_0_OR_GREATER
        // Android .NET 8+ generates a resource designer in a referenced assembly.
        var referenced = assembly.GetReferencedAssemblies();
        AssemblyName? designerName = null;

        for (var i = 0; i < referenced.Length; i++)
        {
            var an = referenced[i];
            if (an.FullName is not null && an.FullName.StartsWith("_Microsoft.Android.Resource.Designer", StringComparison.Ordinal))
            {
                designerName = an;
                break;
            }
        }

        if (designerName is null)
        {
            throw new InvalidOperationException("Could not locate the Android resource designer assembly.");
        }

        var resourcesAssembly = Assembly.Load(designerName);
        var modules = resourcesAssembly.GetModules();

        Type? resources = null;
        for (var i = 0; i < modules.Length && resources is null; i++)
        {
            var types = modules[i].GetTypes();
            for (var j = 0; j < types.Length; j++)
            {
                if (types[j].Name == "ResourceConstant")
                {
                    resources = types[j];
                    break;
                }
            }
        }

        if (resources is null)
        {
            throw new InvalidOperationException("Could not locate generated resource type 'ResourceConstant'.");
        }
#else
        var modules = assembly.GetModules();
        Type? resources = null;

        for (var i = 0; i < modules.Length && resources is null; i++)
        {
            var types = modules[i].GetTypes();
            for (var j = 0; j < types.Length; j++)
            {
                if (types[j].Name == "Resource")
                {
                    resources = types[j];
                    break;
                }
            }
        }

        if (resources is null)
        {
            throw new InvalidOperationException("Could not locate generated resource type 'Resource'.");
        }
#endif

        var idType = resources.GetNestedType("Id");
        if (idType is null)
        {
            throw new InvalidOperationException("Id is not a valid nested type in the generated resources.");
        }

        var fields = idType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        var dict = new Dictionary<string, int>(fields.Length, StringComparer.InvariantCultureIgnoreCase);

        for (var i = 0; i < fields.Length; i++)
        {
            var f = fields[i];
            if (f.FieldType != typeof(int))
            {
                continue;
            }

            // Generated constants use raw constant values.
            if (f.GetRawConstantValue() is int value)
            {
                dict[f.Name] = value;
            }
        }

        return dict;
    }
}
