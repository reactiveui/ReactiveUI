// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public class FakeCollectionViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string?> _numberAsString;

        public FakeCollectionViewModel(FakeCollectionModel model)
        {
            Model = model;

            this.WhenAny(x => x.Model.SomeNumber, x => x.Value.ToString()).ToProperty(this, x => x.NumberAsString, out _numberAsString);
        }

        public FakeCollectionModel Model { get; protected set; }

        public string? NumberAsString => _numberAsString.Value;
    }
}
