// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// Shows the rest of <see cref="DefaultViewLocator"/> and <see cref="ViewMappingBuilder"/> with a school timetable
/// app: a day view and a week view picked by contract, a staff-only view mapped in and out at sign-in and
/// sign-out, a view resolved from the service locator, and the exception the locator throws when it is missing.
/// A private <see cref="DefaultViewLocator"/> keeps every example's mappings out of the app-wide one <see
/// cref="ViewLocator.GetCurrent"/> returns.
/// </summary>
public static class SchoolTimetableExamples
{
    /// <summary>
    /// <c>Map</c> registers the day view as the default and the week view under a contract; <c>ResolveView</c>
    /// with only a type argument finds each without a view model instance, so the returned view's <c>ViewModel</c>
    /// is left unset.
    /// </summary>
    public static void MapDayAndWeekViewsAndResolveThemWithoutAnInstance()
    {
        DefaultViewLocator locator = new();
        locator.Map<TimetableViewModel, DayTimetableView>();
        locator.Map<TimetableViewModel, WeekTimetableView>(ViewContracts.Week);

        IViewFor<TimetableViewModel>? day = locator.ResolveView<TimetableViewModel>();
        IViewFor<TimetableViewModel>? week = locator.ResolveView<TimetableViewModel>(ViewContracts.Week);

        Console.WriteLine(day?.GetType().Name);
        Console.WriteLine(week?.GetType().Name);
        Console.WriteLine(day?.ViewModel is null);

        // Output:
        // DayTimetableView
        // WeekTimetableView
        // True
    }

    /// <summary>
    /// A teacher's sign-in maps the staff admin view with a factory, once as the default and once under a
    /// contract; signing out unmaps both, so the locator finds nothing for the view model again.
    /// </summary>
    public static void MapATeacherViewAtSignInAndUnmapAtSignOut()
    {
        DefaultViewLocator locator = new();
        TeacherAdminViewModel admin = new("Ms. Ito");

        locator.Map<TeacherAdminViewModel>(static () => new TeacherAdminView());
        locator.Map<TeacherAdminViewModel>(static () => new TeacherAdminView(), ViewContracts.Admin);

        Console.WriteLine(locator.ResolveView(admin)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(admin, ViewContracts.Admin)?.GetType().Name);

        bool unmappedDefault = locator.Unmap<TeacherAdminViewModel>();
        bool unmappedAdmin = locator.Unmap<TeacherAdminViewModel>(ViewContracts.Admin);

        Console.WriteLine(locator.ResolveView(admin)?.GetType().Name ?? "(none)");
        Console.WriteLine(unmappedDefault);
        Console.WriteLine(unmappedAdmin);

        // Output:
        // TeacherAdminView
        // TeacherAdminView
        // (none)
        // True
        // True
    }

    /// <summary>
    /// A <see cref="ViewMappingBuilder"/> maps the week view under a contract, maps the staff admin view by
    /// factory with and without a contract, and maps a profile view and a printable profile view that another
    /// module already registered in the service locator.
    /// </summary>
    public static void MapViewsWithABuilderIncludingFromTheServiceLocator()
    {
        AppLocator.Register(static () => new SchoolProfileView());
        AppLocator.Register(static () => new PrintableProfileView());

        DefaultViewLocator locator = new();
        locator.CreateMappingBuilder()
            .Map<TimetableViewModel, WeekTimetableView>(ViewContracts.Week)
            .Map<TeacherAdminViewModel>(static () => new TeacherAdminView())
            .Map<TeacherAdminViewModel>(static () => new TeacherAdminView(), ViewContracts.Admin)
            .MapFromServiceLocator<SchoolProfileViewModel, SchoolProfileView>()
            .MapFromServiceLocator<SchoolProfileViewModel, PrintableProfileView>(ViewContracts.Print);

        TeacherAdminViewModel admin = new("Ms. Ito");
        SchoolProfileViewModel profile = new("Aiko Tanaka");

        Console.WriteLine(locator.ResolveView<TimetableViewModel>(ViewContracts.Week)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(admin)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(admin, ViewContracts.Admin)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(profile)?.GetType().Name);
        Console.WriteLine(locator.ResolveView(profile, ViewContracts.Print)?.GetType().Name);
        Console.WriteLine(ReferenceEquals(locator.ResolveView(profile)?.ViewModel, profile));

        // Output:
        // WeekTimetableView
        // TeacherAdminView
        // TeacherAdminView
        // SchoolProfileView
        // PrintableProfileView
        // True
    }

    /// <summary>
    /// Building each constructor directly shows the message it produces, the same message
    /// <see cref="ViewLocator.GetCurrent"/> throws with when no <see cref="IViewLocator"/> is registered.
    /// </summary>
    public static void BuildAViewLocatorNotFoundException()
    {
        ViewLocatorNotFoundException defaultMessage = new();
        ViewLocatorNotFoundException customMessage = new("No view locator was registered for the timetable module.");
        InvalidOperationException containerDisposed = new("The module container was disposed.");
        ViewLocatorNotFoundException wrapped = new("The timetable module could not resolve its view locator.", containerDisposed);

        Console.WriteLine(defaultMessage.Message);
        Console.WriteLine(customMessage.Message);
        Console.WriteLine(wrapped.Message);
        Console.WriteLine(wrapped.InnerException?.Message);

        // Output:
        // No IViewLocator is registered. Call RxBindingBuilder.CreateReactiveUIBindingBuilder().WithCoreServices().BuildApp() to register default services.
        // No view locator was registered for the timetable module.
        // The timetable module could not resolve its view locator.
        // The module container was disposed.
    }
}
