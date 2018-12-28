// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// Information about a move between locations within a Reactive collection.
    /// </summary>
    /// <typeparam name="T">The type of item contained in the move.</typeparam>
    internal class MoveInfo<T> : IMoveInfo<T>
    {
        public MoveInfo(IEnumerable<T> movedItems, int from, int to)
        {
            MovedItems = movedItems;
            From = from;
            To = to;
        }

        public IEnumerable<T> MovedItems { get; protected set; }

        public int From { get; protected set; }

        public int To { get; protected set; }
    }
}
