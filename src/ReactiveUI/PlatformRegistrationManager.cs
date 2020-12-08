// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// Class that represents the platform registration for ReactiveUI.
    /// </summary>
    public static class PlatformRegistrationManager
    {
        internal static RegistrationNamespace[] DefaultRegistrationNamespaces { get; } =
            (RegistrationNamespace[])Enum.GetValues(typeof(RegistrationNamespace));

        internal static RegistrationNamespace[] NamespacesToRegister { get; set; } = DefaultRegistrationNamespaces;

        /// <summary>
        /// Set the platform namespaces to register.
        /// This needs to be set before the first call to <see cref="RxApp"/>.
        /// </summary>
        /// <param name="namespaces">The namespaces to register.</param>
        public static void SetRegistrationNamespaces(params RegistrationNamespace[] namespaces) => NamespacesToRegister = namespaces;
    }
}
