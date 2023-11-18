// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReactiveUI.Tests;

/// <summary>
/// Project.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
[DataContract]
public class Project : ReactiveObject
{
      private string? _name;

      /// <summary>
      /// Gets or sets the name.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      [DataMember]
      [JsonRequired]
      public string? Name
      {
          get => _name;
          set => this.RaiseAndSetIfChanged(ref _name, value);
      }
}
