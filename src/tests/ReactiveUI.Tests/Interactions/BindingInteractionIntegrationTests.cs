// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Interactions;

/// <summary>Exercises Binding's task-based interactions from a ReactiveUI consumer.</summary>
public class BindingInteractionIntegrationTests
{
    /// <summary>The request used by handler-order and unhandled cases.</summary>
    private const string Request = "request";

    /// <summary>The output after incrementing the sample input.</summary>
    private const int IncrementedValue = 2;

    /// <summary>Verifies a synchronous handler supplies the task's output.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Handle_RegisteredHandler_ReturnsOutput()
    {
        var interaction = new Interaction<string, bool>();
        using var registration = interaction.RegisterHandler(static context => context.SetOutput(context.Input == "accept"));

        await Assert.That(await interaction.Handle("accept")).IsTrue();
    }

    /// <summary>Verifies later handlers take precedence until their registration is disposed.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Handle_DisposedLatestHandler_UsesEarlierHandler()
    {
        var interaction = new Interaction<string, string>();
        using var first = interaction.RegisterHandler(static context => context.SetOutput("first"));
        var latest = interaction.RegisterHandler(static context => context.SetOutput("latest"));

        await Assert.That(await interaction.Handle(Request)).IsEqualTo("latest");
        latest.Dispose();
        await Assert.That(await interaction.Handle(Request)).IsEqualTo("first");
    }

    /// <summary>Verifies an asynchronous handler can complete the interaction.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Handle_AsyncHandler_ReturnsOutput()
    {
        var interaction = new Interaction<int, int>();
        using var registration = interaction.RegisterHandler(static async context =>
        {
            await Task.Yield();
            context.SetOutput(context.Input + 1);
        });

        await Assert.That(await interaction.Handle(1)).IsEqualTo(IncrementedValue);
    }

    /// <summary>Verifies an interaction without a handling registration faults its task.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Handle_NoHandler_Throws()
    {
        var interaction = new Interaction<string, bool>();

        await Assert.That(async () => await interaction.Handle(Request))
            .Throws<UnhandledInteractionException<string, bool>>();
    }
}
