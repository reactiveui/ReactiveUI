// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms
{
    public class FakeWinformsView : Control, IViewFor<FakeWinformViewModel>
    {
        public FakeWinformsView()
        {
            Property1 = new Button();
            Property2 = new Label();
            Property3 = new TextBox();
            Property4 = new RichTextBox();
            BooleanProperty = new CheckBox();
            SomeDouble = new TextBox();
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeWinformViewModel?)value;
        }

        public FakeWinformViewModel? ViewModel { get; set; }

        public Button Property1 { get; }

        public Label Property2 { get; }

        public TextBox Property3 { get; }

        public RichTextBox Property4 { get; }

        public CheckBox BooleanProperty { get; }

        public TextBox SomeDouble { get; }
    }
}
