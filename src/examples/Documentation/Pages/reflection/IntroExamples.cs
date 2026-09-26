// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>Explains why this page's project is the one exception that does not publish as NativeAOT.</summary>
public static class IntroExamples
{
    /// <summary>Every member below is <c>[RequiresUnreferencedCode]</c> or <c>[RequiresDynamicCode]</c>, so this project runs on the JIT instead.</summary>
    public static void ExplainWhyThisProjectRunsOnTheJit()
    {
        Console.WriteLine("This project runs with the JIT, not Native AOT: every member below uses reflection.");

        // Output:
        // This project runs with the JIT, not Native AOT: every member below uses reflection.
    }
}
