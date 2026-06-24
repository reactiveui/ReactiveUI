// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.Builder;
using ReactiveUI.Builder.BlazorServer.Components;
using ReactiveUI.Builder.BlazorServer.Services;
using ReactiveUI.Builder.BlazorServer.ViewModels;
using Splat.Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_ = builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

_ = builder.Services.AddHostedService<ReactiveUiAppHostedService>();

// Per-circuit (per tab) screen/bootstrapper:
_ = builder.Services.AddScoped<IScreen, AppBootstrapper>();

_ = builder.Services.AddSingleton<AppLifetimeCoordinator>();

// This line connects Splat and standard Microsoft DI together
builder.Services.UseMicrosoftDependencyResolver();

_ = RxAppBuilder.CreateReactiveUIBuilder()
    .WithBlazor()
    .WithMessageBus()
    .WithViewsFromAssembly(typeof(Program).Assembly)
    .BuildApp();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

_ = app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

_ = app.UseHttpsRedirection();

_ = app.UseAntiforgery();

_ = app.MapStaticAssets();

_ = app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
