// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// The plug-in's view for <see cref="ReadingChallengeViewModel"/>. It stands in for a view from a library built without
/// the source generator: <see cref="ExcludeFromViewRegistrationAttribute"/> keeps it out of the generated view lookup,
/// and the plug-in registers it with the service locator only.
/// </summary>
[ExcludeFromViewRegistration]
[DebuggerDisplay("ReadingChallengeView ViewModel = {ViewModel}")]
public sealed class ReadingChallengeView : ReactiveUserControl<ReadingChallengeViewModel>;
