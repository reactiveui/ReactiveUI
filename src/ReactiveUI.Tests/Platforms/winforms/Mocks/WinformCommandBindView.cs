// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms
{
    public class WinformCommandBindView : IViewFor<WinformCommandBindViewModel>
    {
        public WinformCommandBindView()
        {
            Command1 = new Button();
            Command2 = new CustomClickableControl();
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (WinformCommandBindViewModel?)value;
        }

        public WinformCommandBindViewModel? ViewModel { get; set; }

        public Button Command1 { get; protected set; }

        public CustomClickableControl Command2 { get; protected set; }
    }
}
