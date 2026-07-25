# Repository working agreement

## Multi-targeting analysis

- Before changing package versions, conditional source inclusion, or target-framework-specific code, read the complete repository `Directory.Build.props`, `Directory.Build.targets`, `Directory.Packages.props`, nested `Directory.Build.*` files, and relevant imported `.props`/`.targets` files.
- Derive the supported TFM matrix from those files and the project conditions. Do not infer repository intent from the current host OS or from the TFMs exercised by a single local build.
- Preserve Windows, macOS, iOS, Mac Catalyst, tvOS, Android, .NET Framework, and plain .NET build paths even when they cannot all execute on the current host. Use MSBuild target-platform conditions for source inclusion when a file is valid only for specific TFMs.
