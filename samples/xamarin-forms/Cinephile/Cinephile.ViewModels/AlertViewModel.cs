// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Cinephile.ViewModels
{
    public class AlertViewModel
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ButtonText { get; private set; }

        public AlertViewModel(string title, string description, string buttonText)
        {
            Title = title;
            Description = description;
            ButtonText = buttonText;
        }
    }
}
