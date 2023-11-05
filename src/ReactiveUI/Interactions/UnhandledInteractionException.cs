// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Indicates that an interaction has gone unhandled.
/// </summary>
/// <typeparam name="TInput">
/// The type of the interaction's input.
/// </typeparam>
/// <typeparam name="TOutput">
/// The type of the interaction's output.
/// </typeparam>
[Serializable]
public class UnhandledInteractionException<TInput, TOutput> : Exception
{
    [field: NonSerialized]
    private readonly Interaction<TInput, TOutput>? _interaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="interaction">The interaction that doesn't have a input handler.</param>
    /// <param name="input">The input into the interaction.</param>
    public UnhandledInteractionException(Interaction<TInput, TOutput> interaction, TInput input)
        : this("Failed to find a registration for a Interaction.")
    {
        _interaction = interaction;
        Input = input;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    public UnhandledInteractionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="message">A message about the exception.</param>
    public UnhandledInteractionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="message">A message about the exception.</param>
    /// <param name="innerException">Any other exception that caused the issue.</param>
    public UnhandledInteractionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The serialization context.</param>
    protected UnhandledInteractionException(SerializationInfo info, StreamingContext context)
        : base(info, context) =>
        Input = (TInput)info.GetValue(nameof(Input), typeof(TInput))!;

    /// <summary>
    /// Gets the interaction that was not handled.
    /// </summary>
    public Interaction<TInput, TOutput>? Interaction => _interaction;

    /// <summary>
    /// Gets the input for the interaction that was not handled.
    /// </summary>
    public TInput Input { get; } = default!;

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(nameof(Input), Input);
        base.GetObjectData(info, context);
    }
}