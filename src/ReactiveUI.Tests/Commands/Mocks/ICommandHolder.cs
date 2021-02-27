// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A ReactiveObject which hosts a command.
    /// </summary>
    public class ICommandHolder : ReactiveObject
    {
        private ICommand? _theCommand;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand? TheCommand
        {
            get => _theCommand;
            set => this.RaiseAndSetIfChanged(ref _theCommand, value);
        }
    }
}
