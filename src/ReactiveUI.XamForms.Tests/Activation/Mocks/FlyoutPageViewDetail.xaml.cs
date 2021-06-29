// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReactiveUI.XamForms.Tests.Activation.Mocks
{
    /// <summary>
    /// FlyoutPageView Detail.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPageViewDetail : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutPageViewDetail"/> class.
        /// </summary>
        public FlyoutPageViewDetail()
        {
            InitializeComponent();
        }
    }
}
