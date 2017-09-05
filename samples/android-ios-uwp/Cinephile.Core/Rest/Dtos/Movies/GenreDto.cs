// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
namespace Cinephile.Core.Rest.Dtos.Movies
{
    public class GenreDto
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get; set;
        }

        public GenreDto()
        {
        }
    }
}
