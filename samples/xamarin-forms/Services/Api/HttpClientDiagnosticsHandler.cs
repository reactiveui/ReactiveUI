// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Services.Api
{
    [DebuggerStepThrough]
    public class HttpClientDiagnosticsHandler : DelegatingHandler
    {
        private static ILogger logger = Log.ForContext<HttpClientDiagnosticsHandler>();


        public HttpClientDiagnosticsHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        public HttpClientDiagnosticsHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
                var totalElapsedTime = Stopwatch.StartNew();

                logger.Debug("Request: {Request}", request);
                if (request.Content != null) {
                    var content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.Debug("Request Content: {Content}", content);
                }

                var responseElapsedTime = Stopwatch.StartNew();
                var response = await base.SendAsync(request, cancellationToken);

                logger.Debug("Response: {Response}", response);
                if (response.Content != null) {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.Debug("Response Content: {Content}", content);
                }

                responseElapsedTime.Stop();
                logger.Debug("Response elapsed time: {ElapsedMilliseconds} ms", responseElapsedTime.ElapsedMilliseconds);

                totalElapsedTime.Stop();
                logger.Debug("Total elapsed time: {ElapsedMilliseconds} ms", totalElapsedTime.ElapsedMilliseconds);

                return response;
        }
    }
}
