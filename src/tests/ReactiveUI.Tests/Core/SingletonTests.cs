// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI singleton classes.
/// </summary>
public class SingletonTests
{
    /// <summary>
    ///     Tests SingletonDataErrorsChangedEventArgs.Value static property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingletonDataErrorsChangedEventArgs_Value_HasCorrectPropertyName()
    {
        // Act & Assert
        await Assert.That(SingletonDataErrorsChangedEventArgs.Value).IsNotNull();
        await Assert.That(SingletonDataErrorsChangedEventArgs.Value.PropertyName).IsEqualTo("Value");
    }

    /// <summary>
    ///     Tests SingletonPropertyChangedEventArgs.ErrorMessage static property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingletonPropertyChangedEventArgs_ErrorMessage_HasCorrectPropertyName()
    {
        // Act & Assert
        await Assert.That(SingletonPropertyChangedEventArgs.ErrorMessage).IsNotNull();
        await Assert.That(SingletonPropertyChangedEventArgs.ErrorMessage.PropertyName).IsEqualTo("ErrorMessage");
    }

    /// <summary>
    ///     Tests SingletonPropertyChangedEventArgs.HasErrors static property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingletonPropertyChangedEventArgs_HasErrors_HasCorrectPropertyName()
    {
        // Act & Assert
        await Assert.That(SingletonPropertyChangedEventArgs.HasErrors).IsNotNull();
        await Assert.That(SingletonPropertyChangedEventArgs.HasErrors.PropertyName).IsEqualTo("HasErrors");
    }

    /// <summary>
    ///     Tests SingletonPropertyChangedEventArgs.Value static property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SingletonPropertyChangedEventArgs_Value_HasCorrectPropertyName()
    {
        // Act & Assert
        await Assert.That(SingletonPropertyChangedEventArgs.Value).IsNotNull();
        await Assert.That(SingletonPropertyChangedEventArgs.Value.PropertyName).IsEqualTo("Value");
    }
}
