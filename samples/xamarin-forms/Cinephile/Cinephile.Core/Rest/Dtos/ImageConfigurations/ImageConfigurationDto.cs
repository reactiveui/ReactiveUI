// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Cinephile.Core.Rest.Dtos.ImageConfigurations
{
    /// <summary>
    /// Contains information about the image configuration.
    /// </summary>
    public class ImageConfigurationDto
    {
        /// <summary>
        /// Gets or sets the image data.
        /// </summary>
        [JsonProperty(PropertyName ="images")]
        public ImagesDto Images { get; set; }

        /// <summary>
        /// Gets or sets the change keys.
        /// </summary>
        [JsonProperty(PropertyName = "change_keys")]
        [SuppressMessage("Design", "CA2227: Change to be read-only by removing the property setter.", Justification = "Used in DTO object.")]
        public IList<string> ChangeKeys { get; set; }
    }
}
