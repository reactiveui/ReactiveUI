// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Contains contextual information for an interaction.
/// </summary>
/// <remarks>
/// <para>
/// Instances of this class are passed into interaction handlers. The <see cref="Input"/> property exposes
/// the input to the interaction, whilst the <see cref="SetOutput"/> method allows a handler to provide the
/// output.
/// </para>
/// </remarks>
/// <typeparam name="TInput">
/// The type of the interaction's input.
/// </typeparam>
/// <typeparam name="TOutput">
/// The type of the interaction's output.
/// </typeparam>
public interface IInteractionContext<out TInput, in TOutput>
{
    /// <summary>
    /// Gets the input for the interaction.
    /// </summary>
    TInput Input { get; }

    /// <summary>
    /// Gets a value indicating whether the interaction is handled. That is, whether the output has been set.
    /// </summary>
    bool IsHandled { get; }

    /// <summary>
    /// Sets the output for the interaction.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    void SetOutput(TOutput output);
}
