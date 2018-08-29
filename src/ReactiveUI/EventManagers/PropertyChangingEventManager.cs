// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
#if !NET_461
    internal class PropertyChangingEventManager : WeakEventManager<INotifyPropertyChanging, PropertyChangingEventHandler, PropertyChangingEventArgs>
    {
    }
#endif
}
