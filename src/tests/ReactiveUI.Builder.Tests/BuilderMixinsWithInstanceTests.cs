// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Comprehensive coverage for all BuilderMixins.WithInstance overloads and the corresponding
/// <see cref="ReactiveUIBuilder"/> instance methods.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BuilderMixinsWithInstanceTests
{
    private static readonly Type[] ServiceTypes =
    [
        typeof(InstanceService01),
        typeof(InstanceService02),
        typeof(InstanceService03),
        typeof(InstanceService04),
        typeof(InstanceService05),
        typeof(InstanceService06),
        typeof(InstanceService07),
        typeof(InstanceService08),
        typeof(InstanceService09),
        typeof(InstanceService10),
        typeof(InstanceService11),
        typeof(InstanceService12),
        typeof(InstanceService13),
        typeof(InstanceService14),
        typeof(InstanceService15),
        typeof(InstanceService16)
    ];

    [TestCaseSource(nameof(ExtensionWithInstanceMethods))]
    public void Extension_methods_invoke_actions_when_current_exists(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        AssertInvocationInvokesAction(method, useExtension: true);
    }

    [TestCaseSource(nameof(ExtensionWithInstanceMethods))]
    public void Extension_methods_ignore_actions_when_current_is_null(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        AssertInvocationSkipsWhenCurrentNull(method, useExtension: true);
    }

    [TestCaseSource(nameof(BuilderWithInstanceMethods))]
    public void Builder_methods_invoke_actions_when_current_exists(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        AssertInvocationInvokesAction(method, useExtension: false);
    }

    [TestCaseSource(nameof(BuilderWithInstanceMethods))]
    public void Builder_methods_ignore_actions_when_current_is_null(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        AssertInvocationSkipsWhenCurrentNull(method, useExtension: false);
    }

    private static IEnumerable<TestCaseData> ExtensionWithInstanceMethods() =>
        typeof(BuilderMixins)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "WithInstance" && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(IReactiveUIInstance))
            .OrderBy(m => m.GetGenericArguments().Length)
            .Select(m => new TestCaseData(m).SetName($"Extension_WithInstance_{m.GetGenericArguments().Length}_Types"));

    private static IEnumerable<TestCaseData> BuilderWithInstanceMethods() =>
        typeof(ReactiveUIBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.Name == "WithInstance" && m.GetParameters().Length == 1)
            .OrderBy(m => m.GetGenericArguments().Length)
            .Select(m => new TestCaseData(m).SetName($"Builder_WithInstance_{m.GetGenericArguments().Length}_Types"));

    private static void AssertInvocationInvokesAction(MethodInfo openMethod, bool useExtension)
    {
        var typeArguments = GetTypeArguments(openMethod);

        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var expected = RegisterServices(resolver, typeArguments);

        builder.WithCoreServices().Build();

        var closedMethod = openMethod.MakeGenericMethod(typeArguments);
        var captured = new List<object?[]>();
        var action = CreateActionDelegate(typeArguments, values => captured.Add(values));

        InvokeWithInstanceMethod(closedMethod, builder, action, useExtension);

        Assert.That(captured, Has.Count.EqualTo(1), openMethod.Name);
        Assert.That(captured[0], Is.EqualTo(expected));
    }

    private static void AssertInvocationSkipsWhenCurrentNull(MethodInfo openMethod, bool useExtension)
    {
        var typeArguments = GetTypeArguments(openMethod);

        AppBuilder.ResetBuilderStateForTests();
        using var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, current: null);
        builder.WithCoreServices();

        var closedMethod = openMethod.MakeGenericMethod(typeArguments);
        var invocationCount = 0;
        var action = CreateActionDelegate(typeArguments, _ => invocationCount++);

        InvokeWithInstanceMethod(closedMethod, builder, action, useExtension);

        Assert.That(invocationCount, Is.Zero, openMethod.Name);
    }

    private static object?[] RegisterServices(IMutableDependencyResolver resolver, Type[] typeArguments)
    {
        var instances = new object?[typeArguments.Length];
        for (var i = 0; i < typeArguments.Length; i++)
        {
            var instance = Activator.CreateInstance(typeArguments[i]) ?? throw new InvalidOperationException("Service instance cannot be null");
            resolver.RegisterConstant(instance, typeArguments[i]);
            instances[i] = instance;
        }

        return instances;
    }

    private static void InvokeWithInstanceMethod(MethodInfo method, IReactiveUIInstance instance, Delegate action, bool useExtension)
    {
        var result = useExtension
            ? method.Invoke(null, [instance, action])
            : method.Invoke(instance, [action]);

        Assert.That(result, Is.SameAs(instance));
    }

    private static Type[] GetTypeArguments(MethodInfo method)
    {
        var count = method.GetGenericArguments().Length;
        return [.. ServiceTypes.Take(count)];
    }

    private static Delegate CreateActionDelegate(Type[] parameterTypes, Action<object?[]> callback)
    {
        var parameters = parameterTypes.Select(Expression.Parameter).ToArray();
        var arrayInit = Expression.NewArrayInit(typeof(object), parameters.Select(p => Expression.Convert(p, typeof(object))));
        var invokeMethod = typeof(Action<object?[]>).GetMethod(nameof(Action.Invoke)) ?? throw new InvalidOperationException("Unable to find Invoke method for callback");
        var body = Expression.Call(Expression.Constant(callback), invokeMethod, arrayInit);
        var actionType = Expression.GetActionType(parameterTypes);
        return Expression.Lambda(actionType, body, parameters).Compile();
    }

    private sealed class InstanceService01;

    private sealed class InstanceService02;

    private sealed class InstanceService03;

    private sealed class InstanceService04;

    private sealed class InstanceService05;

    private sealed class InstanceService06;

    private sealed class InstanceService07;

    private sealed class InstanceService08;

    private sealed class InstanceService09;

    private sealed class InstanceService10;

    private sealed class InstanceService11;

    private sealed class InstanceService12;

    private sealed class InstanceService13;

    private sealed class InstanceService14;

    private sealed class InstanceService15;

    private sealed class InstanceService16;
}
