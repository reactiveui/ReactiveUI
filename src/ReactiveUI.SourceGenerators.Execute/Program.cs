// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable CA1822 // Mark members as static
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace ReactiveUI.SourceGenerators.Test;

/// <summary>
/// EntryPoint.
/// </summary>
public static class EntryPoint
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static void Main() => new TestClass();
}

/// <summary>
/// TestClass.
/// </summary>
public partial class TestClass : ReactiveObject
{
    [Reactive]
    private int _test1Property;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClass"/> class.
    /// </summary>
    public TestClass()
    {
        InitializeCommands();

        Console.Out.WriteLine(Test1Command);
        Console.Out.WriteLine(Test2Command);
        Console.Out.WriteLine(Test3AsyncCommand);
        Console.Out.WriteLine(Test4AsyncCommand);
        Console.Out.WriteLine(Test5StringToIntCommand);
        Console.Out.WriteLine(Test6ArgOnlyCommand);
        Console.Out.WriteLine(Test7ObservableCommand);
        Console.Out.WriteLine(Test8ObservableCommand);
        Test1Command?.Execute().Subscribe();
        Test2Command?.Execute().Subscribe(r => Console.Out.WriteLine(r));
        Test3AsyncCommand?.Execute().Subscribe();
        Test4AsyncCommand?.Execute().Subscribe(r => Console.Out.WriteLine(r));
        Test5StringToIntCommand?.Execute("100").Subscribe(i => Console.Out.WriteLine(i));
        Test6ArgOnlyCommand?.Execute("Hello World").Subscribe();
        Test7ObservableCommand?.Execute().Subscribe();
        Test8ObservableCommand?.Execute(100).Subscribe(i => Console.Out.WriteLine(i));
    }

    /// <summary>
    /// Test1s this instance.
    /// </summary>
    [ReactiveCommand]
    public void Test1() => Console.Out.WriteLine("Test1");

    /// <summary>
    /// Test2s this instance.
    /// </summary>
    /// <returns>Rectangle.</returns>
    [ReactiveCommand]
    public Rectangle Test2() => default;

    /// <summary>
    /// Test3s the asynchronous.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ReactiveCommand]
    public async Task Test3Async() => await Task.Delay(0);

    /// <summary>
    /// Test4s the asynchronous.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ReactiveCommand]
    public async Task<Rectangle> Test4Async() => await Task.FromResult(new Rectangle(0, 0, 100, 100));

    /// <summary>
    /// Test5s the string to int.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>int.</returns>
    [ReactiveCommand]
    public int Test5StringToInt(string str) => int.Parse(str);

    /// <summary>
    /// Test6s the argument only.
    /// </summary>
    /// <param name="str">The string.</param>
    [ReactiveCommand]
    public void Test6ArgOnly(string str) => Console.Out.WriteLine($">>> {str}");

    /// <summary>
    /// Test7s the observable.
    /// </summary>
    /// <returns>An Observable of Unit.</returns>
    [ReactiveCommand]
    public IObservable<Unit> Test7Observable() => Observable.Return(Unit.Default);

    /// <summary>
    /// Test8s the observable.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns>An Observable of int.</returns>
    [ReactiveCommand]
    public IObservable<double> Test8Observable(int i) => Observable.Return(i + 10.0);
}

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
