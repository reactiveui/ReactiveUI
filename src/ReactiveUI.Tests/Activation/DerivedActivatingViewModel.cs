// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reactive.Disposables;

namespace ReactiveUI.Tests
{
    public class DerivedActivatingViewModel : ActivatingViewModel
    {
        public int IsActiveCountAlso { get; protected set; }

        public DerivedActivatingViewModel()
        {
            this.WhenActivated(d =>
            {
                IsActiveCountAlso++;
                d(Disposable.Create(() => IsActiveCountAlso--));
            });
        }
    }
}
