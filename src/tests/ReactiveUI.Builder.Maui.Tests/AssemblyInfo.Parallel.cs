// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Ensures all tests in this assembly run sequentially rather than in parallel.
// Required because MAUI infrastructure (especially on Windows) uses thread-affine resources
// like DispatcherQueue and WindowsXamlManager that cannot be safely shared across concurrent tests.
[assembly: NotInParallel]
