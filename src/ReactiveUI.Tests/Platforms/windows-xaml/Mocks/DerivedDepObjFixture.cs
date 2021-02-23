// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class DerivedDepObjFixture : DepObjFixture
    {
        public static readonly DependencyProperty AnotherTestStringProperty =
            DependencyProperty.Register("AnotherTestString", typeof(string), typeof(DerivedDepObjFixture), new PropertyMetadata(null));

        public string AnotherTestString
        {
            get => (string)GetValue(AnotherTestStringProperty);
            set => SetValue(AnotherTestStringProperty, value);
        }
    }
}
