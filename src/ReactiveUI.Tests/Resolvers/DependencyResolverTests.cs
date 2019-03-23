// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public sealed class DependencyResolverTests : IDisposable
    {
        private readonly IDependencyResolver _resolver;

        public DependencyResolverTests()
        {
            _resolver = new ModernDependencyResolver();
            _resolver.InitializeSplat();
            _resolver.InitializeReactiveUI();
            _resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        [Fact]
        public void AllDefaultServicesShouldBeRegistered()
        {
            using (_resolver.WithResolver())
            {
                foreach (var shouldRegistered in GetServicesThatShouldBeRegistered())
                {
                    IEnumerable<object> resolvedServices = _resolver.GetServices(shouldRegistered.Key);
                    Assert.Equal(shouldRegistered.Value.Count, resolvedServices.Count());
                    foreach (Type implementationType in shouldRegistered.Value)
                    {
                        var isRegistered = resolvedServices.Any(rs => rs.GetType() == implementationType);
                        Assert.Equal(true, isRegistered);
                    }
                }
            }
        }

        public void Dispose()
        {
            _resolver?.Dispose();
        }

        private static Dictionary<Type, List<Type>> GetServicesThatShouldBeRegistered()
        {
            Dictionary<Type, List<Type>> serviceTypeToImplementationTypes = new Dictionary<Type, List<Type>>();

            new Registrations().Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            new PlatformRegistrations().Register((factory, serviceType) =>
            {
                if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                {
                    implementationTypes = new List<Type>();
                    serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                }

                implementationTypes.Add(factory().GetType());
            });

            var typeNames = new[]
            {
                "ReactiveUI.XamForms.Registrations, ReactiveUI.XamForms",
                "ReactiveUI.Winforms.Registrations, ReactiveUI.Winforms",
                "ReactiveUI.Wpf.Registrations, ReactiveUI.Wpf"
            };

            typeNames.ForEach(typeName => GetRegistrationsForPlatform(typeName, serviceTypeToImplementationTypes));

            return serviceTypeToImplementationTypes;
        }

        private static void GetRegistrationsForPlatform(string typeName, Dictionary<Type, List<Type>> serviceTypeToImplementationTypes)
        {
            Type platformRegistrationsType = Type.GetType(typeName);
            if (platformRegistrationsType != null)
            {
                var platformRegistrations = Activator.CreateInstance(platformRegistrationsType);
                System.Reflection.MethodInfo register = platformRegistrationsType.GetMethod("Register");
                var registerParameter = new Action<Func<object>, Type>((factory, serviceType) =>
                {
                    if (serviceTypeToImplementationTypes.TryGetValue(serviceType, out List<Type> implementationTypes) == false)
                    {
                        implementationTypes = new List<Type>();
                        serviceTypeToImplementationTypes.Add(serviceType, implementationTypes);
                    }

                    implementationTypes.Add(factory().GetType());
                });

                register.Invoke(platformRegistrations, new object[] { registerParameter });
            }
        }
    }
}
