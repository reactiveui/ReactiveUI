// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Splat;

namespace ReactiveUI.Drawing
{
    /// <summary>
    /// Splat Drawing platform registrations.
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

#if !NETSTANDARD && !NETCOREAPP2_0
            registerFunction(() => new PlatformBitmapLoader(), typeof(IBitmapLoader));
#endif
        }
    }
}
