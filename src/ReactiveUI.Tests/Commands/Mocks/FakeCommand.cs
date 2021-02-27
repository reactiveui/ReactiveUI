// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A fake command that can be executed as part of a test.
    /// </summary>
    public class FakeCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCommand"/> class.
        /// </summary>
        public FakeCommand()
        {
            CanExecuteParameter = default;
            ExecuteParameter = default;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gets the can execute parameter.
        /// </summary>
        public object? CanExecuteParameter { get; private set; }

        /// <summary>
        /// Gets the execute parameter.
        /// </summary>
        public object? ExecuteParameter { get; private set; }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns>
        ///   <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(object? parameter)
        {
            CanExecuteParameter = parameter;
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(object? parameter) => ExecuteParameter = parameter;

        /// <summary>
        /// Notifies the can execute changed.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void NotifyCanExecuteChanged(EventArgs e) => CanExecuteChanged?.Invoke(this, e);
    }
}
