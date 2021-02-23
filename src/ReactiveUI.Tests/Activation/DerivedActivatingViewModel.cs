// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;

namespace ReactiveUI.Tests
{
    public class DerivedActivatingViewModel : ActivatingViewModel
    {
        public DerivedActivatingViewModel() =>
            this.WhenActivated(d =>
            {
                IsActiveCountAlso++;
                d(Disposable.Create(() => IsActiveCountAlso--));
            });

        public int IsActiveCountAlso { get; protected set; }
    }
}
