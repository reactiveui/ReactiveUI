// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    public interface INotifyPropertyChanging
    {
        event PropertyChangingEventHandler PropertyChanging;
    }
}

#if PORTABLE || NETFX_CORE
#endif

// vim: tw=120 ts=4 sw=4 et :
