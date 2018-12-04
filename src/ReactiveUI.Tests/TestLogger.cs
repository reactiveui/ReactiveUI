// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace ReactiveUI.Tests
{
    public class TestLogger : ILogger
    {
        public TestLogger()
        {
            Messages = new List<Tuple<string, LogLevel>>();
            Level = LogLevel.Debug;
        }

        public List<Tuple<string, LogLevel>> Messages { get; }

        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            Messages.Add(Tuple.Create(message, logLevel));
        }
    }
}
