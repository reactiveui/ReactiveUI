// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
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
    /// <summary>
    /// Helper utility to handle dispatcher in tests.
    /// </summary>
    public static class DispatcherUtilities
    {
        /// <summary>
        /// Makes the dispatcher perform the events to keep it running.
        /// </summary>
        public static void DoEvents()
        {
#if !NETFX_CORE
            DispatcherFrame frame = new();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
#endif
        }

        /// <summary>
        /// Gets the frame to exit.
        /// </summary>
        /// <param name="f">Unused frame object..</param>
        /// <returns>Unused return value.</returns>
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
