// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using Example.Models;
using Example.Models.Interfaces;
using ReactiveUI;

namespace Example.Client.ViewModels
{
    public class IndexViewModel : ReactiveObject, IDisposable
    {
        private readonly ObservableAsPropertyHelper<bool> _loading;
        private readonly CompositeDisposable _disposables = new();
        private Todo? _todo;
        private bool _disposedValue;

        public IndexViewModel(ITodoRepository todoRepository)
        {
            LoadTodo = ReactiveCommand.CreateFromTask<int>(
                async (id, cancellationToken) =>
                {
                    Todo = null!;
                    Todo = await todoRepository.GetTodoAsync(id, cancellationToken.Token);
                    return Unit.Default;
                });

            _loading = LoadTodo.IsExecuting
                .ToProperty(this, x => x.Loading, scheduler: RxApp.MainThreadScheduler);
        }

        public ReactiveCommand<int, Unit> LoadTodo { get; }

        public Todo? Todo
        {
            get => _todo;
            private set => this.RaiseAndSetIfChanged(ref _todo, value);
        }

        public bool Loading => _loading.Value;

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
                    _loading.Dispose();
                    _disposables.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
