﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FluentAssertions;

using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public sealed class DependencyResolverTests
    {
        /// <summary>
        /// Gets RegistrationNamespaces.
        /// </summary>
        public static IEnumerable<object[]> NamespacesToRegister =>
            new List<object[]>
            {
                new object[] { new[] { RegistrationNamespace.XamForms } },
                new object[] { new[] { RegistrationNamespace.Winforms } },
                new object[] { new[] { RegistrationNamespace.Wpf } },
                new object[] { new[] { RegistrationNamespace.Uno } },
                new object[] { new[] { RegistrationNamespace.Blazor } },
                new object[] { new[] { RegistrationNamespace.Drawing } },
                new object[]
                {
                    new[]
                    {
                        RegistrationNamespace.XamForms,
                        RegistrationNamespace.Wpf
                    }
                },
                new object[]
                {
                    new[]
                    {
                        RegistrationNamespace.Blazor,
                        RegistrationNamespace.XamForms,
                        RegistrationNamespace.Wpf
                    }
                }
            };

        [Fact]
        public void AllDefaultServicesShouldBeRegistered()
        {
            var resolver = GenerateResolver();
            using (resolver.WithResolver())
            {
                foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(PlatformRegistrationManager.DefaultRegistrationNamespaces))
                {
                    IEnumerable<object> resolvedServices = resolver.GetServices(shouldRegistered.Key);
                    Assert.Equal(shouldRegistered.Value.Count, resolvedServices.Count());
                    foreach (Type implementationType in shouldRegistered.Value)
                    {
                        resolvedServices
                            .Any(rs => rs.GetType() == implementationType)
                            .Should().BeTrue();
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(NamespacesToRegister))]
        public void AllDefaultServicesShouldBeRegisteredPerRegistrationNamespace(IEnumerable<RegistrationNamespace> namespacesToRegister)
        {
            var resolver = GenerateResolver();
            using (resolver.WithResolver())
            {
                var namespaces = namespacesToRegister.ToArray();

                resolver.InitializeReactiveUI(namespaces);

                var registeredService = GetServicesThatShouldBeRegistered(namespaces);

                foreach (var shouldRegistered in registeredService)
                {
                    IEnumerable<object> resolvedServices = resolver.GetServices(shouldRegistered.Key);

                    foreach (Type implementationType in shouldRegistered.Value)
                    {
                        resolvedServices
                            .Any(rs => rs.GetType() == implementationType)
                            .Should().BeTrue();
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(NamespacesToRegister))]
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Not in NET472")]
        public void RegisteredNamespacesShouldBeRegistered(IEnumerable<RegistrationNamespace> namespacesToRegister)
        {
            var resolver = GenerateResolver();
            using (resolver.WithResolver())
            {
                var namespaces = namespacesToRegister.ToArray();

                resolver.InitializeReactiveUI(namespaces);

                foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(namespaces))
                {
                    IEnumerable<object> resolvedServices = resolver.GetServices(shouldRegistered.Key);

                    resolvedServices
                        .Select(x => x.GetType()?.AssemblyQualifiedName ?? string.Empty)
                        .Any(registeredType => !string.IsNullOrEmpty(registeredType) && PlatformRegistrationManager.DefaultRegistrationNamespaces.Except(namespacesToRegister).All(x => !registeredType.Contains(x.ToString())))
                        .Should().BeTrue();
                }
            }
        }

        private static IEnumerable<string> GetServiceRegistrationTypeNames(
            IEnumerable<RegistrationNamespace> registrationNamespaces)
        {
            foreach (RegistrationNamespace registrationNamespace in registrationNamespaces)
            {
                if (registrationNamespace == RegistrationNamespace.Wpf)
                {
                    yield return "ReactiveUI.Wpf.Registrations, ReactiveUI.Wpf";
                }

                if (registrationNamespace == RegistrationNamespace.XamForms)
                {
                    yield return "ReactiveUI.XamForms.Registrations, ReactiveUI.XamForms";
                }

                if (registrationNamespace == RegistrationNamespace.Winforms)
                {
                    yield return "ReactiveUI.Winforms.Registrations, ReactiveUI.Winforms";
                }
            }
        }

        private static Dictionary<Type, List<Type>> GetServicesThatShouldBeRegistered(IReadOnlyList<RegistrationNamespace> onlyNamespaces)
        {
            Dictionary<Type, List<Type>> serviceTypeToImplementationTypes = new Dictionary<Type, List<Type>>();

            new Registrations().Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            new PlatformRegistrations().Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            var typeNames = GetServiceRegistrationTypeNames(onlyNamespaces);

            typeNames.ForEach(typeName => GetRegistrationsForPlatform(typeName, serviceTypeToImplementationTypes));

            return serviceTypeToImplementationTypes;
        }

        private static ModernDependencyResolver GenerateResolver()
        {
            var resolver = new ModernDependencyResolver();
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            return resolver;
        }

        private static void GetRegistrationsForPlatform(string typeName, Dictionary<Type, List<Type>> serviceTypeToImplementationTypes)
        {
            var platformRegistrationsType = Type.GetType(typeName);
            if (platformRegistrationsType != null)
            {
                var platformRegistrations = Activator.CreateInstance(platformRegistrationsType);
                var register = platformRegistrationsType.GetMethod("Register");
                var registerParameter = new Action<Func<object>, Type>((factory, serviceType) =>
                {
                    if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes) == false)
                    {
                        implementationTypes = new List<Type>();
                        serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                    }

                    implementationTypes.Add(factory().GetType());
                });

                register?.Invoke(platformRegistrations, new object[] { registerParameter });
            }
        }
    }
}
