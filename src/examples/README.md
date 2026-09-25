# Examples

| Folder | What it holds |
|--------|---------------|
| `Documentation/Pages/<page>` | One console project per page of the ReactiveUI handbook. Each snippet on a page comes from its project. |
| `Documentation/Common` | The apps the pages share: a to-do list backed by a store, and a GitHub repository search. Controls are console stand-ins for a button, text box, label and list. |
| `ReactiveUI.Samples.*`, `ReactiveUI.Builder.*` | Complete apps for WPF, WinForms, MAUI and Blazor. |

Every documentation project runs. Each example prints what it does, and the `// Output:` comment under it shows what it
prints, so you can compare the two:

```bash
dotnet run --project src/examples/Documentation/Pages/commands
```

| Page | Shows |
|------|-------|
| `view-models` | `RaiseAndSetIfChanged`, `WhenAnyValue`, `ObservableForProperty`, output properties with `ToProperty`, and a search box that queries as the user types. |
| `commands` | Creating commands, `CanExecute`, `ThrownExceptions`, `InvokeCommand`, combined commands, cancelling, and `BindCommand`. |
| `data-binding` | `Bind`, `OneWayBind`, conversions while binding, and `BindTo`. |
| `when-activated` | `WhenActivated` in a view model and in a view, and reloading each time a screen is shown again. |
| `interactions` | Asking the view a question with `Interaction`, handler order, unanswered questions, and `BindInteraction`. |
| `view-location` | Finding a view by its view model, contracts, view modules, and wrapping the view locator. |
| `routing` | Navigating, going back, resetting the stack, and finding the view for the current page. |

## Property observation and binding

`WhenAnyValue`, `ObservableForProperty`, `Bind`, `OneWayBind`, `BindTo`, `BindCommand`, `BindInteraction` and the view
locator come from [ReactiveUI.Binding](https://github.com/reactiveui/ReactiveUI.Binding.SourceGenerators). Its source
generator writes each binding and view lookup at compile time, so they need no reflection and are safe to trim. The
ReactiveUI package imports the `ReactiveUI.Binding` namespaces for you, so these calls need no `using` directive.

The documentation projects reference the ReactiveUI project, which does not import those namespaces the way the package
does, so `Documentation/Directory.Build.props` imports them for each page.
