// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reactive;

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Observable sequence that records subscription lifetimes and timestamped notification messages sent to observers.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public interface ITestableObservable<T> : IObservable<T>
    {
        /// <summary>
        /// Gets a list of all the subscriptions to the observable sequence, including their lifetimes.
        /// </summary>
        IList<Subscription> Subscriptions { get; }

        /// <summary>
        /// Gets the recorded timestamped notification messages that were sent by the observable sequence to its observers.
        /// </summary>
        IList<Recorded<Notification<T>>> Messages { get; }
    }
}
