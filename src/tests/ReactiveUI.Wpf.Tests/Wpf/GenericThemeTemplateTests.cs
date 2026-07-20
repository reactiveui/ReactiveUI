// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Verifies that the WPF assembly ships its own default theme dictionary (themes/generic.xaml) so that
/// <see cref="TransitioningContentControl"/> and its <see cref="ViewModelViewHost"/> subclass resolve a default
/// control template. Both controls set their default style key to <see cref="TransitioningContentControl"/> and the
/// assembly declares its theme via <c>ThemeInfo(SourceAssembly)</c>, so the framework looks for the template in the
/// same assembly. When the theme dictionary is absent the resolved template is null and hosted content never enters
/// the visual tree, leaving the control blank.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class GenericThemeTemplateTests
{
    /// <summary>The template part name of the container element.</summary>
    private const string PartContainerName = "PART_Container";

    /// <summary>The template part name of the current content presentation site.</summary>
    private const string PartCurrentContentName = "PART_CurrentContentPresentationSite";

    /// <summary>The template part name of the previous image site.</summary>
    private const string PartPreviousImageName = "PART_PreviousImageSite";

    /// <summary>Verifies that a view-model view host picks up the default template from the packaged theme.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHostResolvesDefaultTemplateFromTheme()
    {
        var host = new ViewModelViewHost();

        var template = RealizeDefaultTemplate(host);

        using (Assert.Multiple())
        {
            await Assert.That(template).IsNotNull();
            await Assert.That(template!.FindName(PartContainerName, host)).IsTypeOf<Grid>();
            await Assert.That(template.FindName(PartCurrentContentName, host)).IsTypeOf<ContentPresenter>();
            await Assert.That(template.FindName(PartPreviousImageName, host)).IsTypeOf<Image>();
        }
    }

    /// <summary>Verifies that the base transitioning content control picks up the default template from the packaged theme.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TransitioningContentControlResolvesDefaultTemplateFromTheme()
    {
        var control = new TransitioningContentControl();

        var template = RealizeDefaultTemplate(control);

        using (Assert.Multiple())
        {
            await Assert.That(template).IsNotNull();
            await Assert.That(template!.FindName(PartContainerName, control)).IsTypeOf<Grid>();
            await Assert.That(template.FindName(PartCurrentContentName, control)).IsTypeOf<ContentPresenter>();
        }
    }

    /// <summary>
    /// Drives the control through initialization so the framework resolves its theme style from the assembly's
    /// themes/generic.xaml and assigns the default template, then realizes that template's named parts.
    /// </summary>
    /// <param name="control">The control under test.</param>
    /// <returns>The resolved default control template, or <see langword="null"/> when the theme provided none.</returns>
    private static ControlTemplate? RealizeDefaultTemplate(Control control)
    {
        // EndInit raises OnInitialized, which resolves the theme style from the assembly's packaged
        // themes/generic.xaml and assigns the default Template. ApplyTemplate then builds the template's visual
        // tree so its named parts can be located.
        control.BeginInit();
        control.EndInit();
        _ = control.ApplyTemplate();
        return control.Template;
    }
}
