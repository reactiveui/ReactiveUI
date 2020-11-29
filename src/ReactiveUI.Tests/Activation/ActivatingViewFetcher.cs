// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveUI.Tests
{
    public class ActivatingViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view) => view == typeof(ActivatingView) ? 100 : 0;

        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            if (!(view is ActivatingView av))
            {
                throw new ArgumentNullException(nameof(view));
            }

            return av.Loaded.Select(_ => true).Merge(av.Unloaded.Select(_ => false));
        }
    }
}
