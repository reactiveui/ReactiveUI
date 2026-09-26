# ReactiveUI.Device.Tests

This app runs ReactiveUI's Android and iOS platform code on a real emulator or simulator. The other test projects run
on the build machine, where that code cannot load.

The app has two heads:

- **Android** (`net11.0-android`). An instrumentation runs every test in the app. `dotnet test --device` starts it,
  streams each result back and writes a TRX report on the host.
- **iOS** (`net11.0-ios`). The app delegate runs every test, writes a TRX report and an exit-code file, and exits. It
  builds on Windows and macOS. It runs only on a macOS simulator.

The tests use TUnit on Microsoft.Testing.Platform, like the rest of the repository.

## Run the tests

The `device-tests.cs` script in [reactiveui/actions-common](https://github.com/reactiveui/actions-common/blob/main/scripts/device-tests.cs)
does the whole run: it boots the emulator or simulator, builds and runs the app, collects the logs and shuts the
device down. CI runs the same script through `.github/workflows/device-tests.yml`.

1. Clone `reactiveui/actions-common` next to this repository.
2. From the repository root, run the leg you want:

   ```bash
   # Android: Linux, macOS or Windows with the Android SDK (ANDROID_HOME) and hardware virtualization
   dotnet run --file ../actions-common/scripts/device-tests.cs -- android --project src/tests/ReactiveUI.Device.Tests/ReactiveUI.Device.Tests.csproj

   # iOS: macOS with Xcode and an iOS simulator runtime
   dotnet run --file ../actions-common/scripts/device-tests.cs -- ios --project src/tests/ReactiveUI.Device.Tests/ReactiveUI.Device.Tests.csproj
   ```

3. Read the results in `device-test-results/<platform>`: the TRX report, `logcat.txt` or `console.log`, and the
   emulator or simulator log.

The script exits with 0 when every test passes. The header of `device-tests.cs` lists every option and exit code.

With an emulator already running, you can also run the Android head directly from `src`:

```bash
dotnet test --project tests/ReactiveUI.Device.Tests/ReactiveUI.Device.Tests.csproj -f net11.0-android --device emulator-5554 --report-trx
```

## What the tests cover

| Area | Android | iOS |
|---|---|---|
| Main-thread sequencer | `HandlerSequencer` | `NSRunloopSequencer` |
| Platform registrations | `WithAndroidX` | `WithPlatformServices` |
| Orientation | `PlatformOperations` | `PlatformOperations` |
| Activation | `ReactiveActivity`, `ReactiveAppCompatActivity`, AndroidX `ReactiveFragment`, view holders | `ReactiveViewController` |
| View hosting | `LayoutViewHost`, `ReactiveViewHost`, `ViewMixins.GetViewHost` | `ViewModelViewHost`, `RoutedViewHost` |
| Control wire-up | `ControlFetcherMixins` on views, activities, layout hosts and AndroidX fragments | |
| Suspension | `AutoSuspendHelper`, `BundleSuspensionDriver` | `AppSupportJsonSuspensionDriver` |
| Other | `SharedPreferences` changes, bound services, RecyclerView and pager adapters | `IndexNormalizer` |

## Why the normal CI legs skip it

The solution builds this project on every leg, so a compile break shows up in the normal build. The test step leaves
it out with `!tests/ReactiveUI.Device.Tests/**` in `testProjects`, because it needs a device to run.
