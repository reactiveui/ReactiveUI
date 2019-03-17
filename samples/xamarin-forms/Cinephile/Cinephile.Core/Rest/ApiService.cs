// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using Cinephile.Core.Infrastructure.HttpTools;
using Fusillade;
using Refit;

namespace Cinephile.Core.Rest
{
    /// <summary>
    /// Gets a service which will communicate with a API.
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly Lazy<IRestApiClient> _background;
        private readonly Lazy<IRestApiClient> _userInitiated;
        private readonly Lazy<IRestApiClient> _speculative;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService"/> class.
        /// </summary>
        /// <param name="apiBaseAddress">The URI to the api.</param>
        public ApiService(string apiBaseAddress = null)
        {
            IRestApiClient CreateClient(HttpMessageHandler messageHandler)
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress ?? ApiBaseAddress)
                };

                return RestService.For<IRestApiClient>(client);
            }

            _background = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Background));
#else
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Background));
#endif
            });

            _userInitiated = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.UserInitiated));
#else
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.UserInitiated));
#endif
            });

            _speculative = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Speculative));
#else
                return CreateClient(new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Speculative));
#endif
            });
        }

        /// <summary>
        /// Gets the rest API client in the background.
        /// </summary>
        public IRestApiClient Background => _background.Value;

        /// <summary>
        /// Gets the rest API client which is user initiated.
        /// </summary>
        public IRestApiClient UserInitiated => _userInitiated.Value;

        /// <summary>
        /// Gets the rest API which is speculative.
        /// </summary>
        public IRestApiClient Speculative => _speculative.Value;

        /// <summary>
        /// Gets the API base address and used if no address is passed in.
        /// </summary>
        public string ApiBaseAddress { get; } = "https://api.themoviedb.org/3";
    }
}
