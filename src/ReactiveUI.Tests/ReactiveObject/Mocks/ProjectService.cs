// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Project Service.
    /// </summary>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    public class ProjectService : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        public ProjectService()
        {
            Projects.Add(Guid.NewGuid(), new() { Name = "Dummy1" });
            Projects.Add(Guid.NewGuid(), new() { Name = "Dummy2" });
            Projects.Add(Guid.NewGuid(), new() { Name = "Dummy3" });
            Projects.Add(Guid.NewGuid(), new() { Name = "Dummy4" });
            Projects.Add(Guid.NewGuid(), new() { Name = "Dummy5" });

            ProjectsNullable.Add(Guid.NewGuid(), new() { Name = "Dummy1" });
            ProjectsNullable.Add(Guid.NewGuid(), new() { Name = "Dummy2" });
            ProjectsNullable.Add(Guid.NewGuid(), new() { Name = "Dummy3" });
            ProjectsNullable.Add(Guid.NewGuid(), new() { Name = "Dummy4" });
            ProjectsNullable.Add(Guid.NewGuid(), null);
        }

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        public Dictionary<Guid, Project> Projects { get; } = new();

        /// <summary>
        /// Gets the projects nullable.
        /// </summary>
        /// <value>
        /// The projects nullable.
        /// </value>
        public Dictionary<Guid, Project?> ProjectsNullable { get; } = new();
    }
}
