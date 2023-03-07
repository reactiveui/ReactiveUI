// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI.Tests.Commands.Mocks
{
    internal class MockObject : IDisposable
    {
        private bool _disposedValue;

        public CancellationTokenSource CancellationTokenSource { get; } = new();

        public async Task<bool> StartAsync(int delay)
        {
            for (var i = 0; i < 100; i++)
            {
                try
                {
                    await BackgroundMethodAsync(delay, CancellationTokenSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task StopAsync()
        {
            CancellationTokenSource.Cancel();

            // Wait task to finish
            await Task.Delay(10000);
            CancellationTokenSource.Dispose();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        private static async Task BackgroundMethodAsync(int delay, CancellationToken cancellationToken)
        {
            for (var i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
