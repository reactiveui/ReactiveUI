// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

using NUnit.Framework.Interfaces;

namespace ReactiveUI.Tests;

// run tests on invariant culture to avoid problems e.g with culture specific decimal separator
public sealed class UseInvariantCulture : Attribute, ITestAction
{
    private CultureInfo? _storedCulture;

    /// <inheritdoc/>
    public ActionTargets Targets => ActionTargets.Test;

    /// <inheritdoc/>
    public void BeforeTest(ITest test)
    {
        _storedCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    /// <inheritdoc/>
    public void AfterTest(ITest test)
    {
        if (_storedCulture is not null)
        {
            Thread.CurrentThread.CurrentCulture = _storedCulture;
        }
    }
}
