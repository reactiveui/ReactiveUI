// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Cinephile.Core.Rest.Dtos.Movies
{
    /// <summary>
    /// A data transfer object containing genre information.
    /// </summary>
    public class GenreDto
    {
        /// <summary>
        /// Gets or sets the ID of the genre.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        public string Name
        {
            get; set;
        }
    }
}
