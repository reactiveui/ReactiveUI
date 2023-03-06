// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Models.Interfaces
{
    public interface ITodoRepository : IDisposable
    {
        /// <summary>
        /// Lists the todos asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Enumerable.</returns>
        Task<IEnumerable<Todo>> ListTodosAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the todo asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Todo.</returns>
        Task<Todo> GetTodoAsync(int id, CancellationToken cancellationToken = default);
    }
}
