// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ReactiveUI.XamForms
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        }
    }
}
