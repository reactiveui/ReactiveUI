// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>Application entry point for the library desk sample.</summary>
internal static class Program
{
    /// <summary>
    /// The main entry point. Run with no arguments to open the window interactively, or with <c>--smoke</c> to have
    /// the app drive its own scenario, print what it observed, then close.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        WinFormsBuilderExamples.ConfigureWinForms();
        PlatformOperationsExamples.CheckOrientation();
        RegistrationsExamples.RegisterPlatformServices();
        ActivationForViewFetcherExamples.RankViewTypes();
        ActivationForViewFetcherExamples.ObserveActivation();
        ContentControlBindingHookExamples.AlwaysAllowsTheBindingToProceed();
        ViewModelControlHostExamples.SetTheDefaultForNewHosts();

        if (Array.IndexOf(args, "--smoke") >= 0)
        {
            RunSmokeTest();
            return;
        }

        Application.Run(new LibraryForm());
    }

    /// <summary>
    /// Opens the window, drives the book-loan scenario by hand the way a user would, prints what happened, then
    /// closes. <c>Application.DoEvents</c> pumps the message loop between steps, because ReactiveUI posts every
    /// navigation and binding update through the WinForms main-thread scheduler.
    /// </summary>
    private static void RunSmokeTest()
    {
        LibraryForm form = new();
        form.Show();
        Pump();

        BookListView catalogView = (BookListView)form.BooksHost.Controls[0];
        Book book = catalogView.BooksListBox.Items.Cast<Book>().First();
        catalogView.BooksListBox.SelectedItem = book;
        Pump();
        Console.WriteLine($"Selected book: {book.Title}");

        catalogView.LoanButton.PerformClick();
        Pump();

        LoanFormView loanView = (LoanFormView)form.BooksHost.Controls[0];
        Console.WriteLine("Navigated to the loan form");

        Button memberButton = (Button)loanView.MembersTable.Controls[0];
        Member member = (Member)memberButton.Tag!;
        memberButton.PerformClick();
        Pump();
        Console.WriteLine($"Selected member: {member.Name}");
        Console.WriteLine($"Member card shows: {(form.MemberCardHost.Content as MemberCardView)?.ViewModel?.Member.Name}");

        loanView.ConfirmButton.PerformClick();
        Pump();
        Console.WriteLine($"Book on loan after confirming: {book.IsOnLoan}");

        BookListView catalogAgain = (BookListView)form.BooksHost.Controls[0];
        Console.WriteLine($"Back on the catalog: {!ReferenceEquals(catalogAgain, catalogView)}");
        Console.WriteLine($"Last RoutedControlHost property changed: {form.LastBooksHostPropertyChanged}");
        Console.WriteLine($"Last ViewModelControlHost property changed: {form.LastMemberCardHostPropertyChanged}");

        form.Dispose();
        Environment.Exit(0);
    }

    /// <summary>Pumps the message loop several times, so every posted ReactiveUI continuation gets a chance to run.</summary>
    private static void Pump()
    {
        for (var i = 0; i < 10; i++)
        {
            Application.DoEvents();
        }
    }
}
