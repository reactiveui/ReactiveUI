// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>Shows <c>ListenIncludeLatest</c> replaying the last message to a screen that starts listening after it was sent.</summary>
public static class ListenIncludeLatestExamples
{
    /// <summary>A timetable screen that opens after the announcement was sent still sees it, unlike a plain <c>Listen</c>.</summary>
    public static void ReplayTheLastAnnouncementForALateScreen()
    {
        MessageBus office = new MessageBus();
        office.SendMessage(new Announcement("Sports day is cancelled"));

        List<string> onTimeScreen = [];
        using IDisposable onTimeSubscription = office.Listen<Announcement>().Subscribe(announcement => onTimeScreen.Add(announcement.Text));

        List<string> lateTimetableScreen = [];
        using IDisposable lateSubscription = office.ListenIncludeLatest<Announcement>().Subscribe(announcement => lateTimetableScreen.Add(announcement.Text));

        Console.WriteLine(onTimeScreen.Count);
        Console.WriteLine(lateTimetableScreen.Count);
        Console.WriteLine(lateTimetableScreen[0]);

        // Output:
        // 0
        // 1
        // Sports day is cancelled
    }

    /// <summary>The contract overload of <c>ListenIncludeLatest</c> replays only the last message sent for that contract.</summary>
    public static void ReplayTheLastNoticeForAYearGroup()
    {
        MessageBus office = new MessageBus();
        office.SendMessage(new ClassCancelled("Chemistry"), "Year7");
        office.SendMessage(new ClassCancelled("Art"), "Year8");

        List<string> year7NoticeBoard = [];
        using IDisposable subscription = office.ListenIncludeLatest<ClassCancelled>("Year7").Subscribe(notice => year7NoticeBoard.Add(notice.ClassName));

        Console.WriteLine(year7NoticeBoard[0]);

        // Output:
        // Chemistry
    }
}
