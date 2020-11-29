// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NETFX_CORE
using System.Windows.Threading;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public static class DispatcherUtilities
    {
        public static void DoEvents()
        {
#if !NETFX_CORE
            DispatcherFrame frame = new ();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
#endif
        }

        [SuppressMessage("Design", "CA1801: Parameter never used", Justification = "Used on some platforms.")]
        public static object? ExitFrame(object f)
        {
#if !NETFX_CORE
            ((DispatcherFrame)f).Continue = false;
#endif
            return null;
        }
    }
}
