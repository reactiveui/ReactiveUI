// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;

namespace ReactiveUI.Tests
{
    public class FakeCommand : ICommand
    {
        public FakeCommand()
        {
            CanExecuteParameter = default;
            ExecuteParameter = default;
        }

        public event EventHandler? CanExecuteChanged;

        public object? CanExecuteParameter { get; private set; }

        public object? ExecuteParameter { get; private set; }

        public bool CanExecute(object? parameter)
        {
            CanExecuteParameter = parameter;
            return true;
        }

        public void Execute(object? parameter) => ExecuteParameter = parameter;

        protected virtual void NotifyCanExecuteChanged(EventArgs e) => CanExecuteChanged?.Invoke(this, e);
    }
}
