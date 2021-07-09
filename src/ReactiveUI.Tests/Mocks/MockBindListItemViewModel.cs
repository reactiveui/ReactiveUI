// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    public class MockBindListItemViewModel : ReactiveObject
    {
        private string _name = string.Empty;

        public MockBindListItemViewModel(string name) => Name = name;

        /// <summary>
        /// Gets or sets displayed name of the crumb.
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
