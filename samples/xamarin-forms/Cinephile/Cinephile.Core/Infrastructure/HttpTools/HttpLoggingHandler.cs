// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cinephile.Core.Infrastructure.HttpTools
{
    /// <summary>
    /// A HTTP handler which will log the data going through it.
    /// </summary>
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly string[] _types = { "html", "text", "xml", "json", "txt" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLoggingHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">Any inner handler we want to pass HTTP requests to.</param>
        public HttpLoggingHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        /// <summary>
        /// Sends a request asynchronously.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <param name="cancellationToken">A cancellation token which indicates the send request should end.</param>
        /// <returns>The response from the request.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken).ConfigureAwait(false);

            var req = request;
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} -   Request]";

            Debug.WriteLine($"{msg}========Start==========");
            Debug.WriteLine($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Debug.WriteLine($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync().ConfigureAwait(false);

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join(string.Empty, result.Cast<char>())}");
                }
            }

            var start = DateTime.Now;

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var end = DateTime.Now;

            Debug.WriteLine($"{msg} Duration: {end - start}");
            Debug.WriteLine($"{msg}==========End==========");

            msg = $"[{id} - Response]";
            Debug.WriteLine($"{msg}=========Start=========");

            var resp = response;

            Debug.WriteLine($"{msg} {req.RequestUri.Scheme.ToUpper(CultureInfo.InvariantCulture)}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    end = DateTime.Now;

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join(string.Empty, result.Cast<char>())}");
                    Debug.WriteLine($"{msg} Duration: {end - start}");
                }
            }

            Debug.WriteLine($"{msg}==========End==========");
            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out IEnumerable<string> values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(t => header.Contains(t));
        }
    }
}
