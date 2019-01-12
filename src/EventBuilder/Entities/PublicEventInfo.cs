// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace EventBuilder.Entities
{
    /// <summary>
    /// Represents a public event.
    /// </summary>
    public class PublicEventInfo
    {
        /// <summary>
        /// Gets or sets the name of the public event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the event handler.
        /// </summary>
        public string EventHandlerType { get; set; }

        /// <summary>
        /// Gets or sets the type of the event arguments.
        /// </summary>
        public string EventArgsType { get; set; }

        /// <summary>
        /// Gets or sets the obsolete event information.
        /// </summary>
        public ObsoleteEventInfo ObsoleteEventInfo { get; set; }
    }
}
