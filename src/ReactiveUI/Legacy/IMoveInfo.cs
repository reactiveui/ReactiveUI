// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ReactiveUI.Legacy
{
    public interface IMoveInfo<out T>
    {
        IEnumerable<T> MovedItems { get; }

        int From { get; }

        int To { get; }
    }
}
