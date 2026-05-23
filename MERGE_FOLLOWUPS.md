# Rebase follow-ups

This branch was rebased onto `origin/main` while it carries a large System.Reactive/DynamicData
removal and a fused-sink rewrite. Several `main` PRs changed code in regions this branch had already
rewritten. Where the upstream fix could be cleanly ported onto the rewrite it was done during the
rebase; where the original Rx-based code was wholly replaced, the rewrite was kept and the fix is
listed here to be re-derived and re-validated against the new architecture.

## Ported during the rebase (no further action expected)

- **#4351** interaction async handler scheduling — `Interactions/Interaction.cs`: `YieldToCurrentContext()`
  + `RegisterHandlerCore` re-applied over the `TaskUnitObservable`/`ToUnitObservable` sinks.
- **#4353** suspension persistence (materialize app state before shutdown save) — `Suspension/SuspensionHostExtensions.cs`:
  the persist path now runs the pending one-time load (`RunPendingLoad()`) before `SaveState`.
- **#4349** null load state — `Suspension/SuspensionHostExtensions.cs`: load falls back to
  `CreateNewAppState`/`CreateNewAppStateTyped` when the driver yields `null`.
- **#4358** WPF/winforms design-time activation — `Activation/ViewForMixins.cs`: design-mode early-return
  (no throw) re-applied. The WPF-specific `WpfPropertyBinderImplementation` registration was re-added in
  `ReactiveUI.Wpf/Registrations.cs`.

## Kept the rewrite — upstream fix needs re-validation / re-porting

These files were fully reimplemented on this branch (fused sinks / request objects), so the upstream
fix was entangled with code that no longer exists. The rewrite was taken; re-derive the fix's intent
against the new code and restore the upstream test coverage.

- **#4324** `BindCommand` passes the wrong parameter after the ViewModel is reassigned —
  `Bindings/Command/CommandBinderImplementation.cs` (and `Bindings/Command/CommandBinder.cs`). Check the
  `string.IsNullOrEmpty(toEvent)` rebinding-customizer gate and parameter re-evaluation on rebind.
- **#4350** inherited `DependencyProperty` lookup — `Bindings/Property/PropertyBinderImplementation.cs`,
  `Bindings/Property/PropertyBindingMixins.cs`, `ReactiveUI.Wpf/WpfReactiveUIBuilderExtensions.cs`.
  (`WpfPropertyBinderImplementation` is registered again; confirm the rewritten property binder honours
  inherited DP metadata.)
- **WPF command rebinding dispatcher marshalling** — `ReactiveUI.Wpf/WpfCommandRebindingCustomizer.cs`:
  upstream marshalled `SetValue` to the UI thread via `Dispatcher.BeginInvoke` when off-thread; confirm
  the rewritten customizer preserves that.
- **#4337 mobile cache tuning** — `Builder/ReactiveUIBuilder.cs`: upstream used smaller small/big cache
  limits on `ANDROID`/`IOS`; the rewrite uses fixed `DefaultSmallCacheLimit`/`DefaultBigCacheLimit`
  constants. Decide whether the per-platform tuning should return.

## Tests taken as ours

`tests/ReactiveUI.Tests/InteractionsTest.cs`, `SuspensionHostExtensionsTests.cs`,
`ReactiveUI.Wpf.Tests/Wpf/ValidationBindingWpfTest.cs`,
`ReactiveUI.Blazor.Tests/BlazorReactiveUIBuilderExtensionsTests.cs` kept the branch's versions. Re-add
the upstream test cases that cover the fixes listed above once those are re-ported.
