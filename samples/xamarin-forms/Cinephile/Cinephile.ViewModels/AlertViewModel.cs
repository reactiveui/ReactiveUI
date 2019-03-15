// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Cinephile.ViewModels
{
    /// <summary>
    /// A view model for about an alert.
    /// </summary>
    public class AlertViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="description">A more detailed description about the alert.</param>
        /// <param name="buttonText">The text of the button.</param>
        public AlertViewModel(string title, string description, string buttonText)
        {
            Title = title;
            Description = description;
            ButtonText = buttonText;
        }

        /// <summary>
        /// Gets the title of the alert.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets a detailed description of the alert.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the text of the button.
        /// </summary>
        public string ButtonText { get; }
    }
}
