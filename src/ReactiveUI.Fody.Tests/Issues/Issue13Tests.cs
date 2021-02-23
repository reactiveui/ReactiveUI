// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using Xunit;

namespace ReactiveUI.Fody.Tests.Issues
{
    public class Issue13Tests
    {
        [Fact]
        public void AccessingAChainedObservableAsPropertyOfDoubleDoesntThrow()
        {
            var vm = new VM();
            Assert.Equal(0.0, vm.P2);
        }

        private class VM : ReactiveObject
        {
            public VM()
            {
                Observable.Return(0.0).ToPropertyEx(this, vm => vm.P1);
                this.WhenAnyValue(vm => vm.P1).ToPropertyEx(this, vm => vm.P2);
            }

            [ObservableAsProperty]
            public double P1 { get; }

            [ObservableAsProperty]
            public double P2 { get; }
        }
    }
}
