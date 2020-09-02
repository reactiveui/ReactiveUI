﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Specialized;
using Foundation;

namespace ReactiveUI
{
    /// <summary>
    /// Interface used to extract a common API between <see cref="UIKit.UIView"/>
    /// and <see cref="UIKit.UITableViewCell"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TUIView">The type of the UI view.</typeparam>
    /// <typeparam name="TUIViewCell">The type of the UI view cell.</typeparam>
    internal interface ISectionInformation<TSource, TUIView, TUIViewCell>
    {
        /// <summary>
        /// Gets the collection.
        /// </summary>
        INotifyCollectionChanged? Collection { get; }

        /// <summary>
        /// Gets the cell key selector.
        /// </summary>
        Func<object?, NSString>? CellKeySelector { get; }

        /// <summary>
        /// Gets the initialize cell action.
        /// </summary>
        Action<TUIViewCell>? InitializeCellAction { get; }
    }
}
