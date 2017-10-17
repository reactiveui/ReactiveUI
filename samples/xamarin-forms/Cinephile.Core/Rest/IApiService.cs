// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
namespace Cinephile.Core.Rest
{
    public interface IApiService
    {
        IRestApiClient Speculative { get; }
        IRestApiClient UserInitiated { get; }
        IRestApiClient Background { get; }
    }
}
