// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public sealed class DependencyResolverTests
{
    private static readonly RegistrationNamespace[][] NamespacesToRegister =
    [
        [RegistrationNamespace.Winforms],
        [RegistrationNamespace.Wpf],
        [RegistrationNamespace.Uno],
        [RegistrationNamespace.Blazor],
        [RegistrationNamespace.Drawing],
        [RegistrationNamespace.Blazor, RegistrationNamespace.Wpf]
    ];

    [Test]
    public async Task AllDefaultServicesShouldBeRegistered()
    {
        var resolver = GenerateResolver();
        using (resolver.WithResolver())
        {
            foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(PlatformRegistrationManager.DefaultRegistrationNamespaces))
            {
                var resolvedServices = resolver.GetServices(shouldRegistered.Key);
                await Assert.That(resolvedServices.Count()).IsEqualTo(shouldRegistered.Value.Count);
                foreach (var implementationType in shouldRegistered.Value)
                {
                    await Assert.That(resolvedServices.Any(rs => rs.GetType() == implementationType)).IsTrue();
                }
            }
        }
    }

    [Test]
    public async Task AllDefaultServicesShouldBeRegisteredPerRegistrationNamespace()
    {
        foreach (var namespaces in NamespacesToRegister)
        {
            var resolver = GenerateResolver();
            using (resolver.WithResolver())
            {
                resolver.InitializeReactiveUI(namespaces);

                var registeredService = GetServicesThatShouldBeRegistered(namespaces);

                foreach (var shouldRegistered in registeredService)
                {
                    var resolvedServices = resolver.GetServices(shouldRegistered.Key);

                    foreach (var implementationType in shouldRegistered.Value)
                    {
                        await Assert.That(resolvedServices.Any(rs => rs.GetType() == implementationType)).IsTrue();
                    }
                }
            }
        }
    }

    [Test]
    public async Task RegisteredNamespacesShouldBeRegistered()
    {
        foreach (var namespacesToRegister in NamespacesToRegister)
        {
            var resolver = GenerateResolver();
            using (resolver.WithResolver())
            {
                var namespaces = namespacesToRegister;

                resolver.InitializeReactiveUI(namespaces);

                foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(namespaces))
                {
                    var resolvedServices = resolver.GetServices(shouldRegistered.Key);

                    await Assert.That(resolvedServices
                            .Select(x => x.GetType()?.AssemblyQualifiedName ?? string.Empty)
                            .Any(registeredType =>
                                !string.IsNullOrEmpty(registeredType) &&
                                PlatformRegistrationManager.DefaultRegistrationNamespaces
                                    .Except(namespacesToRegister)
                                    .All(x => !registeredType.Contains(x.ToString())))).IsTrue();
                }
            }
        }
    }

    private static IEnumerable<string> GetServiceRegistrationTypeNames(
        IEnumerable<RegistrationNamespace> registrationNamespaces)
    {
        foreach (var registrationNamespace in registrationNamespaces)
        {
            if (registrationNamespace == RegistrationNamespace.Wpf)
            {
                yield return "ReactiveUI.Wpf.Registrations, ReactiveUI.Wpf";
            }

            if (registrationNamespace == RegistrationNamespace.Winforms)
            {
                yield return "ReactiveUI.Winforms.Registrations, ReactiveUI.Winforms";
            }
        }
    }

    private static Dictionary<Type, List<Type>> GetServicesThatShouldBeRegistered(IReadOnlyList<RegistrationNamespace> onlyNamespaces)
    {
        var serviceTypeToImplementationTypes = new Dictionary<Type, List<Type>>();

        new Registrations().Register((factory, serviceType) =>
        {
            if (!serviceTypeToImplementationTypes.TryGetValue(serviceType!, out var implementationTypes))
            {
                implementationTypes = [];
                serviceTypeToImplementationTypes.Add(serviceType!, implementationTypes);
            }

            implementationTypes.Add(factory()!.GetType());
        });

        new PlatformRegistrations().Register((factory, serviceType) =>
        {
            if (!serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes))
            {
                implementationTypes = [];
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
        if (platformRegistrationsType is not null)
        {
            var platformRegistrations = Activator.CreateInstance(platformRegistrationsType);
            var register = platformRegistrationsType.GetMethod("Register");
            var registerParameter = new Action<Func<object>, Type>((factory, serviceType) =>
            {
                if (!serviceTypeToImplementationTypes.TryGetValue(serviceType, out var implementationTypes))
                {
                    implementationTypes = [];
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            register?.Invoke(
                platformRegistrations,
                [registerParameter]);
        }
    }
}
