// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI attribute types.
/// </summary>
public class AttributeTests
{
    /// <summary>
    ///     Tests that LocalizableAttribute stores false value correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task LocalizableAttribute_FalseValue_StoresFalse()
    {
        // Act
        var attribute = new LocalizableAttribute(false);

        // Assert
        await Assert.That(attribute.IsLocalizable).IsFalse();
    }

    /// <summary>
    ///     Tests that LocalizableAttribute stores true value correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task LocalizableAttribute_TrueValue_StoresTrue()
    {
        // Act
        var attribute = new LocalizableAttribute(true);

        // Assert
        await Assert.That(attribute.IsLocalizable).IsTrue();
    }

    /// <summary>
    ///     Tests that PreserveAttribute can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task PreserveAttribute_Constructor_CreatesInstance()
    {
        // Act
        var attribute = new PreserveAttribute();

        // Assert
        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute).IsTypeOf<PreserveAttribute>();
    }

    /// <summary>
    ///     Tests that SingleInstanceViewAttribute has correct AttributeUsage.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleInstanceViewAttribute_AttributeUsage_IsClass()
    {
        // Arrange
        var attributeType = typeof(SingleInstanceViewAttribute);

        // Act
        var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        await Assert.That(attributeUsage).IsNotNull();
        await Assert.That(attributeUsage!.ValidOn).IsEqualTo(AttributeTargets.Class);
    }

    /// <summary>
    ///     Tests that SingleInstanceViewAttribute can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingleInstanceViewAttribute_Constructor_CreatesInstance()
    {
        // Act
        var attribute = new SingleInstanceViewAttribute();

        // Assert
        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute).IsTypeOf<SingleInstanceViewAttribute>();
    }

    /// <summary>
    ///     Tests that ViewContractAttribute has correct AttributeUsage.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractAttribute_AttributeUsage_IsClass()
    {
        // Arrange
        var attributeType = typeof(ViewContractAttribute);

        // Act
        var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        await Assert.That(attributeUsage).IsNotNull();
        await Assert.That(attributeUsage!.ValidOn).IsEqualTo(AttributeTargets.Class);
    }

    /// <summary>
    ///     Tests that ViewContractAttribute correctly stores contract value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractAttribute_Constructor_StoresContractValue()
    {
        // Arrange
        const string expectedContract = "TestContract";

        // Act
        var attribute = new ViewContractAttribute(expectedContract);

        // Assert
        await Assert.That(attribute.Contract).IsEqualTo(expectedContract);
    }

    /// <summary>
    ///     Tests that ViewContractAttribute handles null contract.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractAttribute_NullContract_StoresNull()
    {
        // Act
        var attribute = new ViewContractAttribute(null!);

        // Assert
        await Assert.That(attribute.Contract).IsNull();
    }
}
