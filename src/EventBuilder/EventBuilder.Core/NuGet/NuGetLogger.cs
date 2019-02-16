// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using NuGet.Common;

namespace EventBuilder.Core.NuGet
{
    /// <summary>
    /// A logger provider for the NuGet clients API.
    /// </summary>
    internal class NuGetLogger : ILogger
    {
        /// <inheritdoc />
        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Warning:
                    Serilog.Log.Warning(data);
                    break;
                case LogLevel.Error:
                    Serilog.Log.Error(data);
                    break;
                case LogLevel.Information:
                    Serilog.Log.Information(data);
                    break;
                case LogLevel.Debug:
                    Serilog.Log.Debug(data);
                    break;
                default:
                    Serilog.Log.Verbose(data);
                    break;
            }
        }

        /// <inheritdoc />
        public void Log(ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        /// <inheritdoc />
        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void LogDebug(string data)
        {
            Serilog.Log.Debug(data);
        }

        /// <inheritdoc />
        public void LogError(string data)
        {
            Serilog.Log.Error(data);
        }

        /// <inheritdoc />
        public void LogInformation(string data)
        {
            Serilog.Log.Information(data);
        }

        /// <inheritdoc />
        public void LogInformationSummary(string data)
        {
            Serilog.Log.Information(data);
        }

        /// <inheritdoc />
        public void LogMinimal(string data)
        {
            Serilog.Log.Information(data);
        }

        /// <inheritdoc />
        public void LogVerbose(string data)
        {
            Serilog.Log.Verbose(data);
        }

        /// <inheritdoc />
        public void LogWarning(string data)
        {
            Serilog.Log.Warning(data);
        }
    }
}
