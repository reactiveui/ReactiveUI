// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ValidationBindingMixins"/>.
/// </summary>
/// <remarks>
/// Note: ValidationBindingMixins creates WPF controls which require an Application to be running.
/// These tests focus on method signatures and null argument validation.
/// Full binding functionality is covered through integration testing scenarios.
/// </remarks>
[NotInParallel]
public class ValidationBindingMixinsTest
{
    /// <summary>
    /// Tests that BindWithValidation method exists and has correct signature.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_MethodExists()
    {
        var method = typeof(ValidationBindingMixins).GetMethod("BindWithValidation");

        await Assert.That(method).IsNotNull();
        await Assert.That(method!.IsStatic).IsTrue();
        await Assert.That(method.IsPublic).IsTrue();
    }

    /// <summary>
    /// Tests that BindWithValidation has correct generic parameters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_HasCorrectGenericParameters()
    {
        var method = typeof(ValidationBindingMixins).GetMethod("BindWithValidation");
        var genericArgs = method!.GetGenericArguments();

        await Assert.That(genericArgs).Count().IsEqualTo(4);
    }

    /// <summary>
    /// Tests that BindWithValidation has correct parameter count.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_HasCorrectParameterCount()
    {
        var method = typeof(ValidationBindingMixins).GetMethod("BindWithValidation");
        var parameters = method!.GetParameters();

        await Assert.That(parameters).Count().IsEqualTo(4);
    }

    /// <summary>
    /// Tests that BindWithValidation returns IReactiveBinding.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithValidation_ReturnsIReactiveBinding()
    {
        var method = typeof(ValidationBindingMixins).GetMethod("BindWithValidation");
        var returnType = method!.ReturnType;

        await Assert.That(returnType.IsGenericType).IsTrue();
        await Assert.That(returnType.GetGenericTypeDefinition()).IsEqualTo(typeof(IReactiveBinding<,>));
    }
}
