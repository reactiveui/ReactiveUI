// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    public class InteractionAncestorViewModel : ReactiveObject
    {
        private InteractionBindViewModel _interactionBindViewModel;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public InteractionAncestorViewModel()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            InteractionViewModel = new InteractionBindViewModel();
        }

        public InteractionBindViewModel InteractionViewModel
        {
            get => _interactionBindViewModel;
            set => this.RaiseAndSetIfChanged(ref _interactionBindViewModel, value);
        }
    }
}
