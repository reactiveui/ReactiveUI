// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Application Fixture.
    /// </summary>
    /// <typeparam name="T">The Application Type.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class ApplicationFixture<T> : IDisposable
        where T : Application
    {
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFixture{T}"/> class.
        /// </summary>
        public ApplicationFixture()
        {
            MockForms.Init();
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public T? AppMock { get; private set; }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        /// <param name="app">The application.</param>
        public void ActivateApp(T app) => AppMock = app;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    AppMock?.Quit();
                    Application.Current = null;
                }

                _disposedValue = true;
            }
        }
    }
}
