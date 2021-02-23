// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xunit.Sdk;
using Xunit.Runners.UI;

namespace Splat.Android.Tests
{
    /// <summary>
    /// Unit Test Runner Activity.
    /// </summary>
    // ReSharper disable UnusedMember.Global
    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity

    // ReSharper restore UnusedMember.Global
    {
        /// <inheritdoc/>
        protected override void OnCreate(Bundle bundle)
        {
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            AddTestAssembly(typeof(ReactiveUI.Tests.RxAppTest).Assembly);

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
