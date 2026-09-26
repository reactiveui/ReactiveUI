// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// The iOS entry point: UIKit starts the app with its delegate, which runs the tests.
UIKit.UIApplication.Main(args, null, typeof(ReactiveUI.Device.Tests.AppDelegate));
