// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A subject is both an observer and an observable: values pushed in via <see cref="IObserver{T}"/> are broadcast to
/// every current <see cref="IObservable{T}"/> subscriber. This is ReactiveUI's owned replacement for
/// <c>System.Reactive.Subjects.ISubject&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T">The element type pushed through the subject.</typeparam>
public interface IReactiveSubject<T> : IObserver<T>, IObservable<T>;
