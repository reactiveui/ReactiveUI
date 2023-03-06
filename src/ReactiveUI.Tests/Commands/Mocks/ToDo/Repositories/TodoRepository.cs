// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Example.Models;
using Example.Models.Interfaces;

namespace Example.Client.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://jsonplaceholder.typicode.com/") };
        private bool _disposedValue;

        public Task<IEnumerable<Todo>> ListTodosAsync(CancellationToken cancellationToken = default) =>
            _http.GetFromJsonAsync<IEnumerable<Todo>>("todos", cancellationToken)!;

        public Task<Todo> GetTodoAsync(int id, CancellationToken cancellationToken = default) =>
            _http.GetFromJsonAsync<Todo>($"todos/{id}", cancellationToken)!;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _http.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
