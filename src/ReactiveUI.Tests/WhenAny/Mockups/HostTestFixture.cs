// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A host test fixture.
    /// </summary>
    public class HostTestFixture : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string?> _ownerName;
        private TestFixture? _Child;
        private OwnerClass? _owner;
        private NonObservableTestFixture? _PocoChild;
        private int _SomeOtherParam;

        public HostTestFixture()
        {
            _ownerName = this.WhenAnyValue(x => x.Owner)
              .WhereNotNull()
              .Select(owner => owner.WhenAnyValue(x => x.Name))
              .Switch()
              .ToProperty(this, x => x.OwnerName);
        }

        /// <summary>
        /// Gets the name of the owner.
        /// </summary>
        /// <value>
        /// The name of the owner.
        /// </value>
        public string? OwnerName => _ownerName.Value;

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public OwnerClass? Owner
        {
            get => _owner;
            set => this.RaiseAndSetIfChanged(ref _owner, value);
        }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public TestFixture? Child
        {
            get => _Child;
            set => this.RaiseAndSetIfChanged(ref _Child, value);
        }

        /// <summary>
        /// Gets or sets the poco child.
        /// </summary>
        public NonObservableTestFixture? PocoChild
        {
            get => _PocoChild;
            set => this.RaiseAndSetIfChanged(ref _PocoChild, value);
        }

        /// <summary>
        /// Gets or sets some other parameter.
        /// </summary>
        public int SomeOtherParam
        {
            get => _SomeOtherParam;
            set => this.RaiseAndSetIfChanged(ref _SomeOtherParam, value);
        }
    }
}
