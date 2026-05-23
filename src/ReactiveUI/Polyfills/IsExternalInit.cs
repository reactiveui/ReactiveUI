// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NET
using System.Diagnostics;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.
/// Modification of Using SimonCropp's polyfill's library.
/// </summary>
[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
internal static class IsExternalInit;

#else
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IsExternalInit))]
#endif
