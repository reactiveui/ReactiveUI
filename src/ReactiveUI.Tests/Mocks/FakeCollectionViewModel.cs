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
    /// <summary>
    /// A mock view model.
    /// </summary>
    public class FakeCollectionViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string?> _numberAsString;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCollectionViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public FakeCollectionViewModel(FakeCollectionModel model)
        {
            Model = model;

            this.WhenAny(x => x.Model.SomeNumber, x => x.Value.ToString()).ToProperty(this, x => x.NumberAsString, out _numberAsString);
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public FakeCollectionModel Model { get; protected set; }

        /// <summary>
        /// Gets the number as string.
        /// </summary>
        public string? NumberAsString => _numberAsString.Value;
    }
}
