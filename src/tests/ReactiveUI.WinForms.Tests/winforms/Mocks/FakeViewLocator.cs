// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

internal class FakeViewLocator : IViewLocator
{
    public Func<Type, IViewFor>? LocatorFunc { get; set; }

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
        where TViewModel : class
    {
        return LocatorFunc?.Invoke(typeof(TViewModel)) as IViewFor<TViewModel>;
    }

    [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance, string? contract = null)
    {
        if (instance is null)
        {
            return null;
        }

        var view = LocatorFunc?.Invoke(instance.GetType());
        if (view is IViewFor viewFor)
        {
            viewFor.ViewModel = instance;
        }

        return view;
    }
}
