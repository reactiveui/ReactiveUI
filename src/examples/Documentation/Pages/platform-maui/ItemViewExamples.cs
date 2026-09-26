// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Graphics;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>Shows the row views for a <c>CollectionView</c> item template: <see cref="ReactiveTextItemView{TViewModel}"/> and <see cref="ReactiveImageItemView{TViewModel}"/>.</summary>
public static class ItemViewExamples
{
    /// <summary><see cref="ReactiveTextItemView{TViewModel}"/> shows a primary and a detail line, each with its own colour.</summary>
    public static void TextItemViewShowsPrimaryAndDetailText()
    {
        RecipeItem soup = RecipeBook.Recipes[0];
        RecipeRowView row = new()
        {
            ViewModel = soup,
            Text = soup.Name,
            Detail = soup.Category,
            TextColor = Colors.Black,
            DetailColor = Colors.Gray,
        };

        Console.WriteLine($"{row.Text} ({row.Detail})");
        Console.WriteLine($"{row.TextColor.Equals(Colors.Black)}, {row.DetailColor.Equals(Colors.Gray)}");

        // Output:
        // Tomato Soup (Starter)
        // True, True
    }

    /// <summary><see cref="ReactiveImageItemView{TViewModel}"/> adds an <c>ImageSource</c> beside the same text and detail.</summary>
    public static void ImageItemViewAddsAPhoto()
    {
        RecipeItem chicken = RecipeBook.Recipes[1];
        ImageSource photo = ImageSource.FromFile("roast-chicken.png");
        RecipeImageRowView row = new()
        {
            ViewModel = chicken,
            ImageSource = photo,
            Text = chicken.Name,
            Detail = chicken.Category,
            TextColor = Colors.Black,
            DetailColor = Colors.Gray,
        };

        Console.WriteLine(ReferenceEquals(row.ImageSource, photo));
        Console.WriteLine($"{row.Text} ({row.Detail})");
        Console.WriteLine($"{row.TextColor.Equals(Colors.Black)}, {row.DetailColor.Equals(Colors.Gray)}");

        // Output:
        // True
        // Roast Chicken (Main)
        // True, True
    }
}
