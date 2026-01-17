// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>
///     Tests for ReactiveUI exception types.
/// </summary>
public class ExceptionTests
{
    /// <summary>
    ///     Tests that UnhandledErrorException can be instantiated with default constructor.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_DefaultConstructor_HasDefaultMessage()
    {
        // Act
        var fixture = new UnhandledErrorException();

        // Assert
        await Assert.That(fixture.Message)
            .IsEqualTo("Exception of type 'ReactiveUI.UnhandledErrorException' was thrown.");
    }

    /// <summary>
    ///     Tests that UnhandledErrorException can be instantiated with message and inner exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_MessageAndInnerException_HasBoth()
    {
        // Arrange
        const string expectedMessage = "We are terribly sorry but a unhandled error occured.";
        const string innerMessage = "Inner Exception added.";
        var innerException = new Exception(innerMessage);

        // Act
        var fixture = new UnhandledErrorException(expectedMessage, innerException);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(fixture.Message).IsEqualTo(expectedMessage);
            await Assert.That(fixture.InnerException).IsNotNull();
            await Assert.That(fixture.InnerException?.Message).IsEqualTo(innerMessage);
        }
    }

    /// <summary>
    ///     Tests that UnhandledErrorException can be instantiated with custom message.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_MessageConstructor_HasCustomMessage()
    {
        // Arrange
        const string expectedMessage = "We are terribly sorry but a unhandled error occured.";

        // Act
        var fixture = new UnhandledErrorException(expectedMessage);

        // Assert
        await Assert.That(fixture.Message).IsEqualTo(expectedMessage);
    }

    /// <summary>
    ///     Tests that ViewLocatorNotFoundException can be instantiated with default constructor.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_DefaultConstructor_HasDefaultMessage()
    {
        // Act
        var fixture = new ViewLocatorNotFoundException();

        // Assert
        await Assert.That(fixture.Message)
            .IsEqualTo("Exception of type 'ReactiveUI.ViewLocatorNotFoundException' was thrown.");
    }

    /// <summary>
    ///     Tests that ViewLocatorNotFoundException can be instantiated with message and inner exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_MessageAndInnerException_HasBoth()
    {
        // Arrange
        const string expectedMessage = "We are terribly sorry but the View Locator was Not Found.";
        const string innerMessage = "Inner Exception added.";
        var innerException = new Exception(innerMessage);

        // Act
        var fixture = new ViewLocatorNotFoundException(expectedMessage, innerException);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(fixture.Message).IsEqualTo(expectedMessage);
            await Assert.That(fixture.InnerException).IsNotNull();
            await Assert.That(fixture.InnerException?.Message).IsEqualTo(innerMessage);
        }
    }

    /// <summary>
    ///     Tests that ViewLocatorNotFoundException can be instantiated with custom message.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_MessageConstructor_HasCustomMessage()
    {
        // Arrange
        const string expectedMessage = "We are terribly sorry but the View Locator was Not Found.";

        // Act
        var fixture = new ViewLocatorNotFoundException(expectedMessage);

        // Assert
        await Assert.That(fixture.Message).IsEqualTo(expectedMessage);
    }
}
