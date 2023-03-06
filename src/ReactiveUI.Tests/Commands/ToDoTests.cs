// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Example.Client.ViewModels;
using Example.Models;
using Example.Models.Interfaces;
using Microsoft.Reactive.Testing;
using NSubstitute;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ToDoTests : ReactiveTest
    {
        private readonly ITodoRepository _mockTodoRepository;

        public ToDoTests() => _mockTodoRepository = Substitute.For<ITodoRepository>();

        [Fact]
        public void LoadTodos_UpdatesIsExecuting() =>
            new TestScheduler().With(async (scheduler) =>
            {
                var vm = new IndexViewModel(_mockTodoRepository);

                scheduler.ScheduleAbsolute(Subscribed + 10, () => vm.LoadTodo.Execute().Subscribe());

                scheduler.ScheduleAbsolute(Subscribed + 110, () => vm.LoadTodo.Execute().Subscribe());

                _mockTodoRepository.GetTodoAsync(default, default)
                    .ReturnsForAnyArgs(async (_) =>
                    {
                        await scheduler.Sleep(TimeSpan.FromTicks(50));
                        return new Todo { Id = 1, UserId = 1, Title = "title", Completed = true };
                    });

                var results = scheduler.Start(() => vm.LoadTodo.IsExecuting, Created, Subscribed, Disposed);

                var expected = new Recorded<Notification<bool>>[]
                {
                    OnNext(Subscribed, false),
                    OnNext(Subscribed + 11, true),
                    OnNext(Subscribed + 61, false),
                    OnNext(Subscribed + 111, true),
                    OnNext(Subscribed + 161, false)
                };
                ReactiveAssert.AreElementsEqual(expected, results.Messages);
                await scheduler.Sleep(TimeSpan.FromTicks(1));
            });

        [Fact]
        public void LoadTodos_UpdatesLoading() =>
            new TestScheduler().With(async (scheduler) =>
            {
                var vm = new IndexViewModel(_mockTodoRepository);

                scheduler.ScheduleAbsolute(Subscribed + 10, () => vm.LoadTodo.Execute().Subscribe());

                scheduler.ScheduleAbsolute(Subscribed + 110, () => vm.LoadTodo.Execute().Subscribe());

                _mockTodoRepository.GetTodoAsync(default, default)
                    .ReturnsForAnyArgs(async (_) =>
                    {
                        await scheduler.Sleep(TimeSpan.FromTicks(50));
                        return new Todo { Id = 1, UserId = 1, Title = "title", Completed = true };
                    });

                var todosObservable = Observable.FromEventPattern<PropertyChangedEventArgs>(vm, nameof(IndexViewModel.PropertyChanged), scheduler)
                    .Where((propertyChangedEvent) => propertyChangedEvent.EventArgs.PropertyName == nameof(IndexViewModel.Loading))
                    .Select((propertyChangedEvent) => ((IndexViewModel?)propertyChangedEvent.Sender)!.Loading);

                var results = scheduler.Start(() => todosObservable, Created, Subscribed, Disposed);

                var expected = new Recorded<Notification<bool>>[]
                {
                    OnNext(Subscribed + 11, true),
                    OnNext(Subscribed + 61, false),
                    OnNext(Subscribed + 111, true),
                    OnNext(Subscribed + 161, false)
                };
                ReactiveAssert.AreElementsEqual(expected, results.Messages);
                await scheduler.Sleep(TimeSpan.FromTicks(1));
            });

        [Fact]
        public void LoadTodos_UpdatesTodos() =>
            new TestScheduler().With(async (scheduler) =>
            {
                var vm = new IndexViewModel(_mockTodoRepository);

                scheduler.ScheduleAbsolute(10, () => vm.LoadTodo.Execute().Subscribe());

                scheduler.ScheduleAbsolute(Subscribed + 10, () => vm.LoadTodo.Execute().Subscribe());

                scheduler.ScheduleAbsolute(Subscribed + 110, () => vm.LoadTodo.Execute().Subscribe());

                _mockTodoRepository.GetTodoAsync(default, default)
                    .ReturnsForAnyArgs(async (_) =>
                    {
                        await scheduler.Sleep(TimeSpan.FromTicks(50));
                        return new Todo { Id = 1, UserId = 1, Title = "title", Completed = true };
                    });

                var todosObservable = Observable.FromEventPattern<PropertyChangedEventArgs>(vm, nameof(IndexViewModel.PropertyChanged), scheduler)
                    .Where((propertyChangedEvent) => propertyChangedEvent.EventArgs.PropertyName == nameof(IndexViewModel.Todo))
                    .Select((propertyChangedEvent) => ((IndexViewModel?)propertyChangedEvent.Sender)!.Todo!);

                var results = scheduler.Start(() => todosObservable, Created, Subscribed, Disposed);

                var expected = new Recorded<Notification<Todo>>[]
                {
                    OnNext(Subscribed + 10, (Todo?)null!),
                    OnNext(Subscribed + 60, new Todo { Id = 1, UserId = 1, Title = "title", Completed = true }),
                    OnNext(Subscribed + 110, (Todo?)null!),
                    OnNext(Subscribed + 160, new Todo { Id = 1, UserId = 1, Title = "title", Completed = true })
                };
                ReactiveAssert.AreElementsEqual(expected, results.Messages);
                await scheduler.Sleep(TimeSpan.FromTicks(1));
            });
    }
}
