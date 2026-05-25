// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A test form that is activatable but does not implement ICanActivate.
/// </summary>
public class TestFormNotCanActivate : Form, IActivatableView;
