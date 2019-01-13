// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace EventBuilder.Entities
{
    /// <summary>
    /// Represents an obsolete event.
    /// </summary>
    public class ObsoleteEventInfo
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the obsolete event is an error.
        /// </summary>
        public bool IsError { get; set; }
    }
}
