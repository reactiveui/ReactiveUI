// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>One row of the members table: a member's name, and the one cell that shows the six classic <c>IReactiveObject</c> members every reactive Apple type carries.</summary>
[System.Diagnostics.DebuggerDisplay("MemberCell")]
public sealed class MemberCell : ReactiveTableViewCell<Member>
{
    /// <summary>The reuse identifier <see cref="MembersPlaceholderViewController"/> registers this cell under.</summary>
    internal static readonly NSString Key = new(nameof(MemberCell));

    /// <summary>Initializes a new instance of the <see cref="MemberCell"/> class from a native handle.</summary>
    /// <param name="handle">The native handle UIKit hands the base constructor.</param>
    public MemberCell(IntPtr handle)
        : base(handle)
    {
        ContentView.AddSubview(NameLabel);
        NameLabel.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            NameLabel.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16),
            NameLabel.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor),
        ]);

        // The classic events fire for any property change; a cell can use them without going through Changed/Changing.
        PropertyChanged += static (_, e) => Console.WriteLine($"MemberCell.{e.PropertyName} changed (classic event).");
        PropertyChanging += static (_, e) => Console.WriteLine($"MemberCell.{e.PropertyName} changing (classic event).");

        _ = this.WhenActivated(d =>
        {
            // Member is an immutable record, not a ReactiveObject, so the label follows ViewModel itself changing
            // (which the cell base class does raise) rather than a OneWayBind on one of its properties.
            d(this.WhenAnyValue(static v => v.ViewModel).Subscribe(member => NameLabel.Text = member?.Name));
            d(Changing.Subscribe(static _ => Console.WriteLine("MemberCell changing.")));
            d(Changed.Subscribe(static _ => Console.WriteLine("MemberCell changed.")));
            d(ThrownExceptions.Subscribe(static error => Console.WriteLine($"MemberCell binding failed: {error.Message}")));
            d(Activated.Subscribe(static _ => Console.WriteLine("MemberCell activated.")));
            d(Deactivated.Subscribe(static _ => Console.WriteLine("MemberCell deactivated.")));
        });
    }

    /// <summary>Gets the member's name. Internal so the binding source generator can observe it.</summary>
    internal UILabel NameLabel { get; } = new() { Font = UIFont.PreferredBody! };

    /// <inheritdoc/>
    public override void PrepareForReuse()
    {
        // Clearing the binding target before reuse should not itself look like a change to any observer, so it runs
        // inside a suppression scope. Only the real value set once the cell is dequeued again should notify.
        using (SuppressChangeNotifications())
        {
            ViewModel = null;
        }

        base.PrepareForReuse();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NameLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
