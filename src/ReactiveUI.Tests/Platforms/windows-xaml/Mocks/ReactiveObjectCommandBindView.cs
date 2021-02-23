// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class ReactiveObjectCommandBindView : ReactiveObject, IViewFor<CommandBindViewModel>
    {
        private CommandBindViewModel? _vm;

        public ReactiveObjectCommandBindView()
        {
            Command1 = new CustomClickButton();
            Command2 = new Image();
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CommandBindViewModel?)value;
        }

        public CommandBindViewModel? ViewModel
        {
            get => _vm;
            set => this.RaiseAndSetIfChanged(ref _vm, value);
        }

        public CustomClickButton Command1 { get; protected set; }

        public Image Command2 { get; protected set; }
    }
}
