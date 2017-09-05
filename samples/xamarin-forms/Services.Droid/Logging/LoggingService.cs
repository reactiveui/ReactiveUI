// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

using Splat;

namespace Services.Droid.Logging
{
    public class LoggingService : ILogger
    {
        public LogLevel Level
        {
            get; set;
        }

        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level)
            {
                return;
            }

            Debug.WriteLine(message);
        }
    }
}
