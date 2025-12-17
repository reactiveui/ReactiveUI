// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
#if !NET8_0_OR_GREATER
[Serializable]
#endif
public class UnhandledInteractionException<TInput, TOutput> : Exception
{
    [field: NonSerialized]
    private readonly Interaction<TInput, TOutput>? _interaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="interaction">The interaction that doesn't have an input handler.</param>
    /// <param name="input">The input into the interaction.</param>
    public UnhandledInteractionException(Interaction<TInput, TOutput> interaction, TInput input)
        : this("Failed to find a registration for an Interaction.")
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

#if !NET8_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledInteractionException{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The serialization context.</param>
#if NET6_0_OR_GREATER || MONOANDROID13_0
    protected UnhandledInteractionException(SerializationInfo info, in StreamingContext context)
#else
    protected UnhandledInteractionException(SerializationInfo info, StreamingContext context)
#endif
        : base(info, context) =>
        Input = (TInput)info.GetValue(nameof(Input), typeof(TInput))!;
#endif

    /// <summary>
    /// Gets the interaction that was not handled.
    /// </summary>
    public Interaction<TInput, TOutput>? Interaction => _interaction;

    /// <summary>
    /// Gets the input for the interaction that was not handled.
    /// </summary>
    public TInput Input { get; } = default!;

#if !NET8_0_OR_GREATER
    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.ArgumentNullExceptionThrowIfNull(nameof(info));

        info.AddValue(nameof(Input), Input);
        base.GetObjectData(info, context);
    }
#endif
}
