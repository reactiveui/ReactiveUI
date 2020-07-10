﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using ReactiveUI.Winforms;
using Xunit;

namespace ReactiveUI.Tests.Winforms
{
    public class CommandBindingTests
    {
        [Fact(Skip = "https://github.com/reactiveui/ReactiveUI/issues/2279")]
        public void CommandBinderBindsToButton()
        {
            var fixture = new CreatesWinformsCommandBinding();
            var cmd = ReactiveCommand.Create<int>(_ => { });
            var input = new Button { };

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
            var commandExecuted = false;
            object ea = null;
            cmd.Subscribe(o =>
            {
                ea = o;
                commandExecuted = true;
            });

            using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
            {
                input.PerformClick();

                Assert.True(commandExecuted);
                Assert.NotNull(ea);
            }
        }

        [Fact(Skip = "https://github.com/reactiveui/ReactiveUI/issues/2279")]
        public void CommandBinderBindsToCustomControl()
        {
            var fixture = new CreatesWinformsCommandBinding();
            var cmd = ReactiveCommand.Create<int>(_ => { });
            var input = new CustomClickableControl { };

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
            var commandExecuted = false;
            object ea = null;
            cmd.Subscribe(o =>
            {
                ea = o;
                commandExecuted = true;
            });

            using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
            {
                input.PerformClick();

                Assert.True(commandExecuted);
                Assert.NotNull(ea);
            }
        }

        [Fact(Skip = "https://github.com/reactiveui/ReactiveUI/issues/2279")]
        public void CommandBinderBindsToCustomComponent()
        {
            var fixture = new CreatesWinformsCommandBinding();
            var cmd = ReactiveCommand.Create<int>(_ => { });
            var input = new CustomClickableComponent { };

            Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
            Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
            var commandExecuted = false;
            object ea = null;
            cmd.Subscribe(o =>
            {
                ea = o;
                commandExecuted = true;
            });

            using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
            {
                input.PerformClick();

                Assert.True(commandExecuted);
                Assert.NotNull(ea);
            }
        }

        [Fact(Skip = "https://github.com/reactiveui/ReactiveUI/issues/2279")]
        public void CommandBinderAffectsEnabledState()
        {
            var fixture = new CreatesWinformsCommandBinding();
            var canExecute = new Subject<bool>();
            canExecute.OnNext(true);

            var cmd = ReactiveCommand.Create(() => { }, canExecute);
            var input = new Button();

            using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
            {
                canExecute.OnNext(true);
                Assert.True(input.Enabled);

                canExecute.OnNext(false);
                Assert.False(input.Enabled);
            }
        }

        [Fact(Skip = "https://github.com/reactiveui/ReactiveUI/issues/2279")]
        public void CommandBinderAffectsEnabledStateForComponents()
        {
            var fixture = new CreatesWinformsCommandBinding();
            var canExecute = new Subject<bool>();
            canExecute.OnNext(true);

            var cmd = ReactiveCommand.Create(() => { }, canExecute);
            var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control

            using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
            {
                canExecute.OnNext(true);
                Assert.True(input.Enabled);

                canExecute.OnNext(false);
                Assert.False(input.Enabled);
            }
        }
    }
}
