// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    public class InteractionBindViewModel : ReactiveObject
    {
        private Interaction<string, bool> _interaction1;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public InteractionBindViewModel()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            Interaction1 = new Interaction<string, bool>();
        }

        public Interaction<string, bool> Interaction1
        {
            get => _interaction1;
            set => this.RaiseAndSetIfChanged(ref _interaction1, value);
        }
    }
}
