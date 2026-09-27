// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The members tab: every library member, as the change-set source a reactive table or collection source reads from.</summary>
[System.Diagnostics.DebuggerDisplay("MembersViewModel Members = {Members.Count}")]
public sealed class MembersViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="MembersViewModel"/> class.</summary>
    /// <param name="hostScreen">The shell that owns the router.</param>
    /// <param name="members">Every library member.</param>
    public MembersViewModel(IScreen hostScreen, IReadOnlyList<Member> members)
    {
        HostScreen = hostScreen;
        Members = members as ObservableCollection<Member> ?? new ObservableCollection<Member>(members);
        MemberCount = members.Count;
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "members";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets every library member. A reactive table or collection source binds to this directly: because it
    /// implements <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>, the source refreshes the
    /// view whenever a member is added or removed.</summary>
    public ObservableCollection<Member> Members { get; }

    /// <summary>Gets how many members the library has. A plain <see cref="int"/> so a one-way binding can observe
    /// it directly, rather than chaining through the non-reactive <see cref="Members"/> list.</summary>
    public int MemberCount { get; }
}
