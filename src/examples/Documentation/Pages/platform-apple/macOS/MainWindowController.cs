// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;
using CoreGraphics;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>
/// The app's single window, hosting the library split view. It derives from
/// <see cref="ReactiveWindowController{TViewModel}"/>, the <see cref="IViewFor{T}"/> form of the non-generic
/// <see cref="ReactiveWindowController"/>, so it has a <c>ViewModel</c> and can use <c>WhenActivated</c>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("MainWindowController")]
public sealed class MainWindowController : ReactiveWindowController<LibraryShellViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MainWindowController"/> class with the library's sample data.</summary>
    public MainWindowController()
        : base(CreateWindow())
    {
    }

    /// <inheritdoc/>
    public override void WindowDidLoad()
    {
        base.WindowDidLoad();

        Member ada = new("M-1", "Ada");
        Member grace = new("M-2", "Grace");
        List<Member> members = [ada, grace];
        List<Book> books =
        [
            new("Pride and Prejudice", "Jane Austen"),
            new("The Hobbit", "J. R. R. Tolkien"),
            new("Dune", "Frank Herbert"),
        ];

        LibraryShellViewModel shell = new();
        BookCatalogViewModel catalog = new(shell, books, members);
        ViewModel = shell;

        Window!.ContentViewController = new LibrarySplitViewController(shell, catalog);

        // ViewModel is set above, so the binding below has a shell to read Title from the moment activation runs.
        _ = this.WhenActivated(d =>
            d(this.OneWayBind(ViewModel, static vm => vm.Title, static v => v.Window!.Title)));
    }

    /// <summary>Creates the window this controller manages. AppKit needs the window before <see cref="WindowDidLoad"/> runs.</summary>
    /// <returns>A borderless-menu, resizable window sized for the catalog.</returns>
    private static NSWindow CreateWindow() =>
        new(
            new CGRect(0, 0, 640, 480),
            NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable,
            NSBackingStore.Buffered,
            false)
        {
            Title = "Library",
        };
}
