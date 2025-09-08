// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.


using System.Linq;

namespace ReactiveUI.Tests;

[TestFixture]
public sealed class DependencyResolverTests
{
    /// <summary>
    /// Gets RegistrationNamespaces.
    /// </summary>
    public static IEnumerable<TestCaseData> NamespacesToRegister =>
        new List<TestCaseData>
        {
            new TestCaseData(new[] { RegistrationNamespace.XamForms }),
            new TestCaseData(new[] { RegistrationNamespace.Winforms }),
            new TestCaseData(new[] { RegistrationNamespace.Wpf }),
            new TestCaseData(new[] { RegistrationNamespace.Uno }),
            new TestCaseData(new[] { RegistrationNamespace.Blazor }),
            new TestCaseData(new[] { RegistrationNamespace.Drawing }),
            new TestCaseData(new[] { RegistrationNamespace.XamForms, RegistrationNamespace.Wpf }),
            new TestCaseData(new[] { RegistrationNamespace.Blazor, RegistrationNamespace.XamForms, RegistrationNamespace.Wpf }),
        };

    [Test]
    public void AllDefaultServicesShouldBeRegistered()
    {
        var resolver = GenerateResolver();
        using (resolver.WithResolver())
        {
            foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(PlatformRegistrationManager.DefaultRegistrationNamespaces))
            {
                var resolvedServices = resolver.GetServices(shouldRegistered.Key);
                Assert.That(resolvedServices.Count(), Is.EqualTo(shouldRegistered.Value.Count));
                foreach (var implementationType in shouldRegistered.Value)
                {
                    resolvedServices
                        .Any(rs => rs.GetType() == implementationType)
                        .Should().BeTrue();
                }
            }
        }
    }

    [Test]
    [TestCaseSource(nameof(NamespacesToRegister))]
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
                var resolvedServices = resolver.GetServices(shouldRegistered.Key);

                foreach (var implementationType in shouldRegistered.Value)
                {
                    resolvedServices
                        .Any(rs => rs.GetType() == implementationType)
                        .Should().BeTrue();
                }
            }
        }
    }

    [Test]
    [TestCaseSource(nameof(NamespacesToRegister))]
    public void RegisteredNamespacesShouldBeRegistered(IEnumerable<RegistrationNamespace> namespacesToRegister)
    {
        var resolver = GenerateResolver();
        using (resolver.WithResolver())
        {
            var namespaces = namespacesToRegister.ToArray();

            resolver.InitializeReactiveUI(namespaces);

            foreach (var shouldRegistered in GetServicesThatShouldBeRegistered(namespaces))
            {
                var resolvedServices = resolver.GetServices(shouldRegistered.Key);

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
        foreach (var registrationNamespace in registrationNamespaces)
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

            register?.Invoke(platformRegistrations,
                             [registerParameter]);
        }
    }
}
