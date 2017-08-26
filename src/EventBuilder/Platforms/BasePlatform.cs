// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    public class BasePlatform : IPlatform
    {
        public BasePlatform()
        {
            Assemblies = new List<string>();
            CecilSearchDirectories = new List<string>();
        }

        public List<string> Assemblies { get; set; }
        public List<string> CecilSearchDirectories { get; set; }
    }
}