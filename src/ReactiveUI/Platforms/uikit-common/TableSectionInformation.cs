// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Class used to extract a common API between <see cref="UITableView"/> and <see cref="UITableViewCell"/>.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
[SuppressMessage("Design", "SST1452:Remove unused type parameters", Justification = "TSource is part of the public generic API and preserves call-site type safety for consumers.")]
public class TableSectionInformation<TSource> : ISectionInformation<UITableViewCell>
{
    /// <inheritdoc/>
    public INotifyCollectionChanged? Collection { get; protected set; }

    /// <inheritdoc/>
    public Action<UITableViewCell>? InitializeCellAction { get; protected set; }

    /// <inheritdoc/>
    public Func<object?, NSString>? CellKeySelector { get; protected set; }

    /// <summary>Gets the size hint.</summary>
    public float SizeHint { get; protected set; }

    /// <summary>Gets or sets the header of this section.</summary>
    /// <value>The header, or null if a header shouldn't be used.</value>
    public TableSectionHeader? Header { get; set; }

    /// <summary>Gets or sets the footer of this section.</summary>
    /// <value>The footer, or null if a footer shouldn't be used.</value>
    public TableSectionHeader? Footer { get; set; }
}
