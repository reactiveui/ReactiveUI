﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

namespace EventBuilder.Entities
{
    public class PublicEventInfo
    {
        public string Name { get; set; }
        public string EventHandlerType { get; set; }
        public string EventArgsType { get; set; }
        public ObsoleteEventInfo ObsoleteEventInfo { get; set; }
    }
}