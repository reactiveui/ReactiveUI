// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using Refit;
using Fusillade;
using Cinephile.Core.Infrastructure.HttpTools;

namespace Cinephile.Core.Rest
{
    public class ApiService : IApiService
    {
        public const string ApiBaseAddress = "https://api.themoviedb.org/3";

        public IRestApiClient Background
        {
            get { return background.Value; }
        }

        public IRestApiClient UserInitiated
        {
            get { return userInitiated.Value; }
        }

        public IRestApiClient Speculative
        {
            get { return speculative.Value; }
        }

        private readonly Lazy<IRestApiClient> background;
        private readonly Lazy<IRestApiClient> userInitiated;
        private readonly Lazy<IRestApiClient> speculative;

        public ApiService(string apiBaseAddress = null)
        {

            Func<HttpMessageHandler, IRestApiClient> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress ?? ApiBaseAddress)
                };

                return RestService.For<IRestApiClient>(client);
            };

            background = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return createClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Background));
#else
                return createClient(new HttpClientHandler(new HttpClientHandler(), Priority.Background)));
#endif
            });

            userInitiated = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return createClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.UserInitiated));
#else
                return createClient(new HttpClientHandler(new HttpClientHandler(), Priority.UserInitiated)));
#endif
            });

            speculative = new Lazy<IRestApiClient>(() =>
            {
#if DEBUG
                return createClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Speculative));
#else
                return createClient(new HttpClientHandler(new HttpClientHandler(), Priority.Speculative)));
#endif
            });
        }
    }
}
