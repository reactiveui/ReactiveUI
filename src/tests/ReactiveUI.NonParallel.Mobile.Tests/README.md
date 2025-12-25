# ReactiveUI.NonParallel.Mobile.Tests

This project contains non-parallel tests for mobile platforms (Android, iOS, macOS).

## Platforms

- **android/** - Android platform tests
- **AndroidX/** - AndroidX-specific tests
- **cocoa/** - iOS and macOS (Cocoa) platform tests

## Why Not in Solution?

This project is not included in the main solution file because:
1. CI/CD runners don't have Android/iOS/macOS build environments configured
2. These tests require platform-specific tooling (Xcode for iOS/macOS, Android SDK)
3. Running these tests requires physical or virtual devices for the respective platforms

## Running Tests

To run these tests locally, you'll need:
- For Android: Android SDK and emulator or physical device
- For iOS/macOS: macOS with Xcode installed

Build and run:
```bash
dotnet test ReactiveUI.NonParallel.Mobile.Tests.csproj --framework net9.0-android
dotnet test ReactiveUI.NonParallel.Mobile.Tests.csproj --framework net9.0-ios
```
