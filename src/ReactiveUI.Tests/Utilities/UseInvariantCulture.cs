// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;
using System.Threading;

using Xunit.Sdk;

namespace ReactiveUI.Tests
{
    // run tests on invariant culture to avoid problems e.g with culture specific decimal separator
    public sealed class UseInvariantCulture : BeforeAfterTestAttribute
    {
        private CultureInfo? _storedCulture;

        /// <inheritdoc/>
        public override void Before(MethodInfo methodUnderTest)
        {
            _storedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        /// <inheritdoc/>
        public override void After(MethodInfo methodUnderTest)
        {
            if (_storedCulture is not null)
            {
                Thread.CurrentThread.CurrentCulture = _storedCulture;
            }
        }
    }
}
