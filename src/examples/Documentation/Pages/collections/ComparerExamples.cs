// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Collections;

/// <summary>Shows <see cref="OrderedComparer{T}"/> and <see cref="ComparerChainingExtensions"/>, which build a multi-key
/// <see cref="IComparer{T}"/> for a leaderboard: highest score first, ties broken by name.</summary>
public static class ComparerExamples
{
    /// <summary><c>OrderByDescending</c> starts the chain, and a plain <c>ThenBy</c> breaks a tie by name.</summary>
    public static void RankLeaderboardByScoreThenName()
    {
        List<Player> leaderboard = [new("Ada", 92), new("Alan", 92), new("Grace", 88), new("Barbara", 75)];
        IComparer<Player> byScoreThenName = OrderedComparer<Player>
            .OrderByDescending(static player => player.Score)
            .ThenBy(static player => player.Name);

        leaderboard.Sort(byScoreThenName);

        foreach (Player player in leaderboard)
        {
            Console.WriteLine($"{player.Name}: {player.Score}");
        }

        // Output:
        // Ada: 92
        // Alan: 92
        // Grace: 88
        // Barbara: 75
    }

    /// <summary>The comparer overload of <c>OrderByDescending</c> and <c>ThenBy</c> compares names without regard to case.</summary>
    public static void RankLeaderboardIgnoringNameCase()
    {
        List<Player> leaderboard = [new("Mike", 95), new("Zoe", 80), new("adam", 80)];
        IComparer<Player> byScoreThenNameIgnoringCase = OrderedComparer<Player>
            .OrderByDescending(static player => player.Score, Comparer<int>.Default)
            .ThenBy(static player => player.Name, StringComparer.OrdinalIgnoreCase);

        leaderboard.Sort(byScoreThenNameIgnoringCase);

        foreach (Player player in leaderboard)
        {
            Console.WriteLine($"{player.Name}: {player.Score}");
        }

        // Output:
        // Mike: 95
        // adam: 80
        // Zoe: 80
    }

    /// <summary><c>OrderBy</c> can start the chain too, with <c>ThenByDescending</c> breaking a tie the other way.</summary>
    public static void RankLeaderboardByNameThenHighestScore()
    {
        List<Player> leaderboard = [new("Sam", 60), new("Ben", 70), new("Sam", 85)];
        IComparer<Player> byNameThenHighestScore = OrderedComparer<Player>
            .OrderBy(static player => player.Name)
            .ThenByDescending(static player => player.Score);

        leaderboard.Sort(byNameThenHighestScore);

        foreach (Player player in leaderboard)
        {
            Console.WriteLine($"{player.Name}: {player.Score}");
        }

        // Output:
        // Ben: 70
        // Sam: 85
        // Sam: 60
    }

    /// <summary>The comparer overload of <c>OrderBy</c> and <c>ThenByDescending</c> also takes an explicit comparer.</summary>
    public static void RankLeaderboardCaseInsensitiveNameThenHighestScore()
    {
        List<Player> leaderboard = [new("SAM", 40), new("Ben", 70), new("sam", 90)];
        IComparer<Player> byNameIgnoringCaseThenHighestScore = OrderedComparer<Player>
            .OrderBy(static player => player.Name, StringComparer.OrdinalIgnoreCase)
            .ThenByDescending(static player => player.Score, Comparer<int>.Default);

        leaderboard.Sort(byNameIgnoringCaseThenHighestScore);

        foreach (Player player in leaderboard)
        {
            Console.WriteLine($"{player.Name}: {player.Score}");
        }

        // Output:
        // Ben: 70
        // sam: 90
        // SAM: 40
    }

    /// <summary><c>OrderedComparer.For</c>, given the leaderboard, infers the row type and returns a builder that can
    /// produce several comparers, one per sort a screen might offer.</summary>
    public static void ChooseALeaderboardSortAtRuntime()
    {
        List<Player> leaderboard = [new("Ada", 92), new("Ben", 65), new("Zack", 99)];
        IComparerBuilder<Player> builder = OrderedComparer.For(leaderboard);

        Console.WriteLine(TopPlayer(leaderboard, builder.OrderByDescending(static player => player.Score)));
        Console.WriteLine(TopPlayer(leaderboard, builder.OrderByDescending(static player => player.Score, Comparer<int>.Default)));
        Console.WriteLine(TopPlayer(leaderboard, builder.OrderBy(static player => player.Name)));
        Console.WriteLine(TopPlayer(leaderboard, builder.OrderBy(static player => player.Name, StringComparer.Ordinal)));

        // Output:
        // Zack
        // Zack
        // Ada
        // Ada
    }

    /// <summary><c>OrderedComparer.For&lt;T&gt;()</c> builds the same kind of builder without needing a sample sequence.</summary>
    public static void BuildComparerForAnEmptyLeaderboard()
    {
        IComparerBuilder<Player> builder = OrderedComparer.For<Player>();
        IComparer<Player> byScoreThenName = builder.OrderByDescending(static player => player.Score).ThenBy(static player => player.Name);

        List<Player> leaderboard = [new("Grace", 81), new("Alan", 81), new("Ada", 92)];
        leaderboard.Sort(byScoreThenName);

        foreach (Player player in leaderboard)
        {
            Console.WriteLine($"{player.Name}: {player.Score}");
        }

        // Output:
        // Ada: 92
        // Alan: 81
        // Grace: 81
    }

    /// <summary>Sorts a copy of the leaderboard with the given comparer and returns the name of the top row.</summary>
    /// <param name="leaderboard">The rows to rank.</param>
    /// <param name="comparer">The comparer to rank them with.</param>
    /// <returns>The name of the top-ranked player.</returns>
    private static string TopPlayer(List<Player> leaderboard, IComparer<Player> comparer)
    {
        List<Player> ranked = [.. leaderboard];
        ranked.Sort(comparer);
        return ranked[0].Name;
    }
}
