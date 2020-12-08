// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests
{
    public class DecoratorModel : BaseModel
    {
        private readonly BaseModel _model;

        // Testing ctor
        public DecoratorModel() => _model = new BaseModel();

        public DecoratorModel(BaseModel baseModel) => _model = baseModel;

        [Reactive]
        public string? SomeCoolNewProperty { get; set; }

        // Works with private fields
        [ReactiveDependency(nameof(_model))]
        public override string? StringProperty { get; set; }

        // Can't be attributed as has additional functionality in the decorated get
        public override int IntProperty
        {
            get => _model.IntProperty * 2;
            set
            {
                _model.IntProperty = value;
                this.RaisePropertyChanged();
            }
        }

        public void UpdateCoolProperty(string coolNewProperty) => SomeCoolNewProperty = coolNewProperty;
    }
}
