// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="UnhandledInteractionException{TInput, TOutput}"/>.
/// </summary>
public class UnhandledInteractionExceptionTest
{
    /// <summary>
    /// Tests that parameterless constructor creates exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_Parameterless_CreatesException()
    {
        var exception = new UnhandledInteractionException<string, int>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.Input).IsNull();
        await Assert.That(exception.Interaction).IsNull();
    }

    /// <summary>
    /// Tests that constructor with message creates exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithMessage_SetsMessage()
    {
        var message = "Test error message";

        var exception = new UnhandledInteractionException<string, int>(message);

        await Assert.That(exception.Message).IsEqualTo(message);
    }

    /// <summary>
    /// Tests that constructor with message and inner exception creates exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithMessageAndInnerException_SetsProperties()
    {
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner");

        var exception = new UnhandledInteractionException<string, int>(message, innerException);

        await Assert.That(exception.Message).IsEqualTo(message);
        await Assert.That(exception.InnerException).IsEqualTo(innerException);
    }

    /// <summary>
    /// Tests that constructor with interaction and input sets properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithInteractionAndInput_SetsProperties()
    {
        var interaction = new Interaction<string, int>();
        var input = "test input";

        var exception = new UnhandledInteractionException<string, int>(interaction, input);

        await Assert.That(exception.Interaction).IsEqualTo(interaction);
        await Assert.That(exception.Input).IsEqualTo(input);
        await Assert.That(exception.Message).Contains("Failed to find a registration");
    }

    /// <summary>
    /// Tests that exception can be thrown and caught.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Exception_CanBeThrownAndCaught()
    {
        var interaction = new Interaction<string, int>();
        var input = "test";

        await Assert.That(() => throw new UnhandledInteractionException<string, int>(interaction, input))
            .Throws<UnhandledInteractionException<string, int>>();
    }

    /// <summary>
    /// Tests that Input property returns the input value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Input_ReturnsInputValue()
    {
        var interaction = new Interaction<int, string>();
        var input = 42;

        var exception = new UnhandledInteractionException<int, string>(interaction, input);

        await Assert.That(exception.Input).IsEqualTo(input);
    }

    /// <summary>
    /// Tests that Interaction property returns the interaction.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Interaction_ReturnsInteraction()
    {
        var interaction = new Interaction<string, int>();
        var input = "test";

        var exception = new UnhandledInteractionException<string, int>(interaction, input);

        await Assert.That(exception.Interaction).IsEqualTo(interaction);
    }
}
