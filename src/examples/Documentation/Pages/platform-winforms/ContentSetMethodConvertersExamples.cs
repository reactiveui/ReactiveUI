// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows <see cref="PanelSetMethodBindingConverter"/> and <see cref="TableContentSetMethodBindingConverter"/>, the
/// set-method converters <c>Bind</c> uses to fill a <see cref="Panel"/>'s or <see cref="TableLayoutPanel"/>'s
/// <c>Controls</c> collection from a source, since neither collection has a normal property setter.
/// </summary>
public static class ContentSetMethodConvertersExamples
{
    /// <summary>The panel converter matches a source that holds <see cref="Control"/>s and a target of <see cref="Control.ControlCollection"/>; nothing else.</summary>
    public static void PanelConverterMatchesAControlCollection()
    {
        PanelSetMethodBindingConverter converter = new();

        Console.WriteLine(converter.GetAffinityForObjects(typeof(List<Label>), typeof(Control.ControlCollection)));
        Console.WriteLine(converter.GetAffinityForObjects(typeof(string), typeof(Control.ControlCollection)));

        // Output:
        // 10
        // 0
    }

    /// <summary>Performing the set clears the panel and adds every control from the new value.</summary>
    public static void PanelConverterReplacesTheControls()
    {
        using Panel panel = new();
        using Label first = new() { Text = "Chicken soup" };
        using Label second = new() { Text = "Bread rolls" };

        PanelSetMethodBindingConverter converter = new();
        converter.PerformSet(panel.Controls, new List<Label> { first, second }, arguments: null);

        Console.WriteLine(panel.Controls.Count);
        Console.WriteLine(((Label)panel.Controls[0]).Text);

        // Output:
        // 2
        // Chicken soup
    }

    /// <summary>The table converter matches the same kind of source, but against a <see cref="TableLayoutControlCollection"/> instead.</summary>
    public static void TableConverterMatchesATableLayoutCollection()
    {
        TableContentSetMethodBindingConverter converter = new();

        Console.WriteLine(converter.GetAffinityForObjects(typeof(List<Label>), typeof(TableLayoutControlCollection)));
        Console.WriteLine(converter.GetAffinityForObjects(typeof(List<Label>), typeof(Control.ControlCollection)));

        // Output:
        // 10
        // 0
    }

    /// <summary>Performing the set clears the table and adds every control from the new value.</summary>
    public static void TableConverterReplacesTheControls()
    {
        using TableLayoutPanel table = new();
        using Label first = new() { Text = "Roast dinner" };

        TableContentSetMethodBindingConverter converter = new();
        converter.PerformSet(table.Controls, new List<Label> { first }, arguments: null);

        Console.WriteLine(table.Controls.Count);
        Console.WriteLine(((Label)table.Controls[0]).Text);

        // Output:
        // 1
        // Roast dinner
    }
}
