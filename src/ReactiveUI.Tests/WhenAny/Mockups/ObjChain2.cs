// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class ObjChain2 : ReactiveObject
{
    private ObjChain3 _model = new();

    public ObjChain3 Model
    {
        get => _model;
        set => this.RaiseAndSetIfChanged(ref _model, value);
    }
}
