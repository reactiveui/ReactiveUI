// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A mock view model.
    /// </summary>
    public class CommandBindViewModel : ReactiveObject
    {
        private ReactiveCommand<int, Unit> _Command1 = null!;
        private ReactiveCommand<Unit, Unit> _Command2 = null!;

        private int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBindViewModel"/> class.
        /// </summary>
        public CommandBindViewModel()
        {
            Command1 = ReactiveCommand.Create<int, Unit>(_ => Unit.Default);
            Command2 = ReactiveCommand.Create(() => { });
            NestedViewModel = new FakeNestedViewModel();
        }

        /// <summary>
        /// Gets or sets the command1.
        /// </summary>
        public ReactiveCommand<int, Unit> Command1
        {
            get => _Command1;
            set => this.RaiseAndSetIfChanged(ref _Command1, value);
        }

        /// <summary>
        /// Gets or sets the command2.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Command2
        {
            get => _Command2;
            set => this.RaiseAndSetIfChanged(ref _Command2, value);
        }

        /// <summary>
        /// Gets or sets the nested view model.
        /// </summary>
        public FakeNestedViewModel NestedViewModel { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}
