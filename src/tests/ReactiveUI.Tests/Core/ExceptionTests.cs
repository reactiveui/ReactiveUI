// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;

/// <summary>Tests for ReactiveUI exception types.</summary>
public class ExceptionTests
{
    /// <summary>Tests that UnhandledErrorException can be instantiated with default constructor.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_DefaultConstructor_HasDefaultMessage()
    {
        // Act
        var fixture = new UnhandledErrorException();

        // Assert
        await Assert.That(fixture.Message).IsEqualTo("Exception of type 'ReactiveUI.UnhandledErrorException' was thrown.");
    }

    /// <summary>Tests that UnhandledErrorException can be instantiated with message and inner exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_MessageAndInnerException_HasBoth()
    {
        // Arrange
        const string ExpectedMessage = "We are terribly sorry but a unhandled error occured.";
        const string InnerMessage = "Inner Exception added.";
        var innerException = new InvalidOperationException(InnerMessage);

        // Act
        var fixture = new UnhandledErrorException(ExpectedMessage, innerException);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(fixture.Message).IsEqualTo(ExpectedMessage);
            await Assert.That(fixture.InnerException).IsNotNull();
            await Assert.That(fixture.InnerException?.Message).IsEqualTo(InnerMessage);
        }
    }

    /// <summary>Tests that UnhandledErrorException can be instantiated with custom message.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledErrorException_MessageConstructor_HasCustomMessage()
    {
        // Arrange
        const string ExpectedMessage = "We are terribly sorry but a unhandled error occured.";

        // Act
        var fixture = new UnhandledErrorException(ExpectedMessage);

        // Assert
        await Assert.That(fixture.Message).IsEqualTo(ExpectedMessage);
    }

    /// <summary>Tests that ViewLocatorNotFoundException can be instantiated with default constructor.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_DefaultConstructor_HasDefaultMessage()
    {
        // Act
        var fixture = new ViewLocatorNotFoundException();

        // Assert
        await Assert.That(fixture.Message).IsEqualTo("Exception of type 'ReactiveUI.ViewLocatorNotFoundException' was thrown.");
    }

    /// <summary>Tests that ViewLocatorNotFoundException can be instantiated with message and inner exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_MessageAndInnerException_HasBoth()
    {
        // Arrange
        const string ExpectedMessage = "We are terribly sorry but the View Locator was Not Found.";
        const string InnerMessage = "Inner Exception added.";
        var innerException = new InvalidOperationException(InnerMessage);

        // Act
        var fixture = new ViewLocatorNotFoundException(ExpectedMessage, innerException);

        using (Assert.Multiple())
        {
            // Assert
            await Assert.That(fixture.Message).IsEqualTo(ExpectedMessage);
            await Assert.That(fixture.InnerException).IsNotNull();
            await Assert.That(fixture.InnerException?.Message).IsEqualTo(InnerMessage);
        }
    }

    /// <summary>Tests that ViewLocatorNotFoundException can be instantiated with custom message.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocatorNotFoundException_MessageConstructor_HasCustomMessage()
    {
        // Arrange
        const string ExpectedMessage = "We are terribly sorry but the View Locator was Not Found.";

        // Act
        var fixture = new ViewLocatorNotFoundException(ExpectedMessage);

        // Assert
        await Assert.That(fixture.Message).IsEqualTo(ExpectedMessage);
    }
}
