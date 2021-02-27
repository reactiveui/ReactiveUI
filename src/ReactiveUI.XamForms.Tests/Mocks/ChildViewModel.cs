// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.XamForms.Tests.Mocks
{
    /// <summary>
    /// The child view model.
    /// </summary>
    public class ChildViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewModel"/> class.
        /// </summary>
        public ChildViewModel() => HostScreen = Locator.Current.GetService<IScreen>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewModel"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ChildViewModel(string value)
            : this() =>
            Value = value;

        /// <inheritdoc/>
        public string? UrlPathSegment => "Child view: " + Value;

        /// <inheritdoc/>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; } = string.Empty;
    }
}
