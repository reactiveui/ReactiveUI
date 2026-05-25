// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="UnhandledInteractionException{TInput, TOutput}" />.
/// </summary>
public class UnhandledInteractionExceptionTest
{
    /// <summary>
    ///     Tests that parameterless constructor creates exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_Parameterless_CreatesException()
    {
        var exception = new UnhandledInteractionException<string, int>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.Input).IsNull();
        await Assert.That(exception.Interaction).IsNull();
    }

    /// <summary>
    ///     Tests that constructor with interaction and input sets properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithInteractionAndInput_SetsProperties()
    {
        var interaction = new Interaction<string, int>();
        const string Input = "test input";

        var exception = new UnhandledInteractionException<string, int>(interaction, Input);

        await Assert.That(exception.Interaction).IsEqualTo(interaction);
        await Assert.That(exception.Input).IsEqualTo(Input);
        await Assert.That(exception.Message).Contains("Failed to find a registration");
    }

    /// <summary>
    ///     Tests that constructor with message creates exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithMessage_SetsMessage()
    {
        const string Message = "Test error message";

        var exception = new UnhandledInteractionException<string, int>(Message);

        await Assert.That(exception.Message).IsEqualTo(Message);
    }

    /// <summary>
    ///     Tests that constructor with message and inner exception creates exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithMessageAndInnerException_SetsProperties()
    {
        const string Message = "Test error message";
        var innerException = new InvalidOperationException("Inner");

        var exception = new UnhandledInteractionException<string, int>(Message, innerException);

        await Assert.That(exception.Message).IsEqualTo(Message);
        await Assert.That(exception.InnerException).IsEqualTo(innerException);
    }

    /// <summary>
    ///     Tests that exception can be thrown and caught.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Exception_CanBeThrownAndCaught()
    {
        var interaction = new Interaction<string, int>();
        const string Input = "test";

        await Assert.That(() => throw new UnhandledInteractionException<string, int>(interaction, Input))
            .Throws<UnhandledInteractionException<string, int>>();
    }

    /// <summary>
    ///     Tests that Input property returns the input value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Input_ReturnsInputValue()
    {
        var interaction = new Interaction<int, string>();
        const int Input = 42;

        var exception = new UnhandledInteractionException<int, string>(interaction, Input);

        await Assert.That(exception.Input).IsEqualTo(Input);
    }

    /// <summary>
    ///     Tests that Interaction property returns the interaction.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Interaction_ReturnsInteraction()
    {
        var interaction = new Interaction<string, int>();
        const string Input = "test";

        var exception = new UnhandledInteractionException<string, int>(interaction, Input);

        await Assert.That(exception.Interaction).IsEqualTo(interaction);
    }
}
