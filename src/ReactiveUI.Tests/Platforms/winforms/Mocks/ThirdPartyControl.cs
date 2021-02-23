// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Forms;

namespace AThirdPartyNamespace
{
    public class ThirdPartyControl : Control
    {
        private string? _value;

        public event EventHandler? ValueChanged;

        public string? Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}
