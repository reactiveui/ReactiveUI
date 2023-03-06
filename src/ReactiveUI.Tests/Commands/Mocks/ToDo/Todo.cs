// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Example.Models
{
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    public class Todo : IEquatable<Todo>
#pragma warning restore CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Title { get; set; }

        public bool Completed { get; set; }

        public bool Equals(Todo? other)
        {
            return Id == other?.Id
                && UserId == other.UserId
                && Title == other.Title
                && Completed == other.Completed;
        }
    }
}
