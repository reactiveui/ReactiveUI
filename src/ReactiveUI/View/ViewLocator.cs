// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ReactiveUI;
using Splat;

namespace ReactiveUI
{
    public static class ViewLocator
    {
        public static IViewLocator Current
        {
            get
            {
                var ret = Locator.Current.GetService<IViewLocator>();
                if (ret == null)
                {
                    throw new Exception("Could not find a default ViewLocator. This should never happen, your dependency resolver is broken");
                }

                return ret;
            }
        }
    }
}
