// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.Maui
{
    /// <summary>
    /// The main registration for common classes for the Splat dependency injection.
    /// We have code that runs reflection through the different ReactiveUI classes
    /// searching for IWantsToRegisterStuff and will register all our required DI
    /// interfaces. The registered items in this classes are common for all Platforms.
    /// To get these registrations after the main ReactiveUI Initialization use the
    /// DependencyResolverMixins.InitializeReactiveUI() extension method.
    /// </summary>
    /// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
    public class Registrations : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            if (registerFunction is null)
            {
                throw new ArgumentNullException(nameof(registerFunction));
            }

            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        }
    }
}
