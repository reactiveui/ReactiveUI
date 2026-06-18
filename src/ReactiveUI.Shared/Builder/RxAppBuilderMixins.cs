// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>Provides extension members for configuring ReactiveUI using the builder pattern.</summary>
public static class RxAppBuilderMixins
{
    /// <summary>Provides builder-creation extension members for <see cref="IMutableDependencyResolver"/>.</summary>
    /// <param name="resolver">The dependency resolver to use.</param>
    extension(IMutableDependencyResolver resolver)
    {
        /// <summary>Creates a ReactiveUI builder with the specified dependency resolver.</summary>
        /// <returns>The ReactiveUI builder instance.</returns>
        public ReactiveUIBuilder CreateReactiveUIBuilder()
        {
            ArgumentExceptionHelper.ThrowIfNull(resolver);

            var readonlyResolver = (resolver as IReadonlyDependencyResolver) ?? AppLocator.Current;
            return new(resolver, readonlyResolver);
        }
    }
}
