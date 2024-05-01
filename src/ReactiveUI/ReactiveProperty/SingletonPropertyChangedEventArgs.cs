// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

internal static class SingletonPropertyChangedEventArgs
{
    public static readonly PropertyChangedEventArgs Value = new(nameof(Value));
    public static readonly PropertyChangedEventArgs HasErrors = new(nameof(INotifyDataErrorInfo.HasErrors));
    public static readonly PropertyChangedEventArgs ErrorMessage = new(nameof(ErrorMessage));
}
