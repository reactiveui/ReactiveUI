// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Tests
{
    [DataContract]
    public class WhenAnyTestFixture : ReactiveObject
    {
        [IgnoreDataMember]
#pragma warning disable SA1401 // Fields should be private
        internal ObservableAsPropertyHelper<int?>? _accountsFound;

#pragma warning restore SA1401 // Fields should be private

        [IgnoreDataMember]
        private AccountService _accountService = new();

        [IgnoreDataMember]
        private ProjectService _projectService = new();

        /// <summary>
        /// Gets or sets the account service.
        /// </summary>
        /// <value>
        /// The account service.
        /// </value>
        [DataMember]
        public AccountService AccountService
        {
            get => _accountService;
            set => this.RaiseAndSetIfChanged(ref _accountService, value);
        }

        /// <summary>
        /// Gets or sets the project service.
        /// </summary>
        /// <value>
        /// The project service.
        /// </value>
        [DataMember]
        public ProjectService ProjectService
        {
            get => _projectService;
            set => this.RaiseAndSetIfChanged(ref _projectService, value);
        }

        /// <summary>
        /// Gets the first three letters of one word.
        /// </summary>
        [IgnoreDataMember]
        public int? AccountsFound => _accountsFound!.Value;
    }
}
