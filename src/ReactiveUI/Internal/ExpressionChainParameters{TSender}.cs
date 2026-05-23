// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Internal;

/// <summary>
/// Bundles the configuration for an <see cref="ExpressionChainSink{TSender, TValue}"/> so the chain engine and its inner
/// sink can be constructed from a single value rather than a long parameter list.
/// </summary>
/// <typeparam name="TSender">The root sender type surfaced on the emitted change.</typeparam>
/// <param name="Source">The root object of the chain.</param>
/// <param name="Expression">The full expression surfaced on the emitted change.</param>
/// <param name="Links">The member-access links of the chain, in order.</param>
/// <param name="BeforeChange">Whether to observe values before they change.</param>
/// <param name="SuppressWarnings">Whether to suppress POCO observation warnings.</param>
/// <param name="SkipInitial">When true, the first raw emission is suppressed.</param>
/// <param name="IsDistinct">When true, consecutive equal leaf values are suppressed.</param>
/// <param name="Notify">Produces the change notifications for a link on a given parent value.</param>
internal readonly record struct ExpressionChainParameters<TSender>(
    TSender? Source,
    Expression? Expression,
    Expression[] Links,
    bool BeforeChange,
    bool SuppressWarnings,
    bool SkipInitial,
    bool IsDistinct,
    Func<object, Expression, bool, bool, IObservable<IObservedChange<object?, object?>>> Notify);
