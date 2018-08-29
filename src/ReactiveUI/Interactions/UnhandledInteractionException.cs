// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// Indicates that an interaction has gone unhandled.
    /// </summary>
    /// <typeparam name="TInput">
    /// The type of the interaction's input.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the interaction's output.
    /// </typeparam>
    public class UnhandledInteractionException<TInput, TOutput> : Exception
    {
        private readonly Interaction<TInput, TOutput> _interaction;
        private readonly TInput _input;

        public UnhandledInteractionException(Interaction<TInput, TOutput> interaction, TInput input)
        {
            _interaction = interaction;
            _input = input;
        }

        /// <summary>
        /// Gets the interaction that was not handled.
        /// </summary>
        public Interaction<TInput, TOutput> Interaction => _interaction;

        /// <summary>
        /// Gets the input for the interaction that was not handled.
        /// </summary>
        public TInput Input => _input;
    }
}
