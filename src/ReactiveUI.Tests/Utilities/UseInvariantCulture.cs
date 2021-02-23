// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Sdk;

namespace ReactiveUI.Tests
{
    // run tests on invariant culture to avoid problems e.g with culture specific decimal separator
    public class UseInvariantCulture : BeforeAfterTestAttribute
    {
        private CultureInfo? _storedCulture;

        public override void Before(MethodInfo methodUnderTest)
        {
            _storedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (_storedCulture is not null)
            {
                Thread.CurrentThread.CurrentCulture = _storedCulture;
            }
        }
    }
}
