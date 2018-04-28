// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Reactive.Disposables;

namespace ReactiveUI.Tests
{
    public class ActivatingViewModel : ReactiveObject, ISupportsActivation
    {
        public ViewModelActivator Activator { get; protected set; }

        public int IsActiveCount { get; protected set; }

        public ActivatingViewModel()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(d => {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });
        }
    }
}
