// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The members tab: a chip strip of every member above a table of the same members, one row each.</summary>
[System.Diagnostics.DebuggerDisplay("MembersPlaceholderViewController")]
public sealed class MembersPlaceholderViewController : ReactiveTableViewController<MembersViewModel>
{
    /// <summary>Gets the horizontal chip strip above the table. Internal so the binding source generator can reach it.</summary>
    internal MemberChipStripView ChipStrip { get; } = new(
        new CGRect(0, 0, 320, 56),
        new UICollectionViewFlowLayout
        {
            ScrollDirection = UICollectionViewScrollDirection.Horizontal,
            ItemSize = new CGSize(48, 48),
            MinimumInteritemSpacing = 12,
            SectionInset = new UIEdgeInsets(8, 16, 8, 16),
        });

    /// <summary>Gets the reactive table source bound in <see cref="ViewDidLoad"/>.</summary>
    internal ReactiveTableViewSource<Member>? Source { get; private set; }

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Members";
        ChipStrip.ViewModel = ViewModel;
        TableView.RegisterClassForCellReuse(typeof(MemberCell), MemberCell.Key);
        TableView.TableHeaderView = ChipStrip;

        _ = this.WhenActivated(d =>
        {
            TableSectionInformation<Member, MemberCell> section = new(
                ViewModel!.Members,
                MemberCell.Key,
                sizeHint: 56F,
                static cell => Console.WriteLine($"Initializing a {cell.GetType().Name}."))
            {
                Header = new TableSectionHeader("Members"),
                Footer = new TableSectionHeader(
                    static () => new UILabel
                    {
                        Text = "Tap a member to see their card.",
                        TextAlignment = UITextAlignment.Center,
                        Font = UIFont.PreferredFootnote!,
                        TextColor = UIColor.SecondaryLabel,
                    },
                    24F),
            };
            IReadOnlyList<TableSectionInformation<Member, MemberCell>> sections = [section];

            d(Signal.Emit(sections).BindTo(TableView, source =>
            {
                Source = source;
                return source.ElementSelected.Subscribe(static item => Console.WriteLine($"Selected member {((Member)item!).Name}."));
            }));

            TableSectionInformation<Member> readBack = Source!.Data[0];
            Console.WriteLine(
                $"{readBack.Header?.Title}: {Source.Data.Count} section(s), row height {readBack.SizeHint}, "
                + $"has a collection {readBack.Collection is not null}, has an init action {readBack.InitializeCellAction is not null}.");
        });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChipStrip.Dispose();
        }

        base.Dispose(disposing);
    }
}
