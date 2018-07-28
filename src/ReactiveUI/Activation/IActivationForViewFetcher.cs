// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// Implement this interface to override how ReactiveUI determines when a
    /// View is activated or deactivated. This is usually only used when porting
    /// ReactiveUI to a new UI framework
    /// </summary>
    public interface IActivationForViewFetcher
    {
        int GetAffinityForView(Type view);
        IObservable<bool> GetActivationForView(IActivatable view);
    }
}
