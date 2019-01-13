// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Tests;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ExampleViewModel : ReactiveObject
    {
    }

    public class AnotherViewModel : ReactiveObject
    {
    }

    public class NeverUsedViewModel : ReactiveObject
    {
    }

    public class SingleInstanceExampleViewModel : ReactiveObject
    {
    }

    public class ViewModelWithWeirdName : ReactiveObject
    {
    }

    public class ExampleWindowViewModel : ReactiveObject
    {
    }

    public class ExampleWindowView : ReactiveWindow<ExampleWindowViewModel>
    {
    }

    public class ExampleView : ReactiveUI.Winforms.ReactiveUserControl<ExampleViewModel>
    {
    }

    public class AnotherView : ReactiveUI.Winforms.ReactiveUserControl<AnotherViewModel>
    {
    }

    [ViewContract("contract")]
    public class ContractExampleView : ReactiveUI.Winforms.ReactiveUserControl<ExampleViewModel>
    {
    }

    [SingleInstanceView]
    public class NeverUsedView : ReactiveUI.Winforms.ReactiveUserControl<NeverUsedViewModel>
    {
        public static int Instances;

        public NeverUsedView()
        {
            Instances++;
        }
    }

    [SingleInstanceView]
    public class SingleInstanceExampleView : ReactiveUI.Winforms.ReactiveUserControl<SingleInstanceExampleViewModel>
    {
        public static int Instances;

        public SingleInstanceExampleView()
        {
            Instances++;
        }
    }

    [ViewContract("contract")]
    [SingleInstanceView]
    public class SingleInstanceWithContractExampleView : ReactiveUI.Winforms.ReactiveUserControl<SingleInstanceExampleViewModel>
    {
        public static int Instances;

        public SingleInstanceWithContractExampleView()
        {
            Instances++;
        }
    }

    public class ViewWithoutMatchingName : ReactiveUI.Winforms.ReactiveUserControl<ViewModelWithWeirdName>
    {
    }

    public sealed class DependencyResolverTests : IDisposable
    {
        private readonly IMutableDependencyResolver _resolver;

        public DependencyResolverTests()
        {
            _resolver = new ModernDependencyResolver();
            _resolver.InitializeSplat();
            _resolver.InitializeReactiveUI();
            _resolver.RegisterViewsForViewModels(GetType().Assembly);
        }

        [WpfFact]
        public void RegisterViewsForViewModelShouldRegisterAllViews()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices<IViewFor<ExampleViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<AnotherViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ExampleWindowViewModel>>());
                Assert.Single(_resolver.GetServices<IViewFor<ViewModelWithWeirdName>>());
            }
        }

        [WpfFact]
        public void RegisterViewsForViewModelShouldIncludeContracts()
        {
            using (_resolver.WithResolver())
            {
                Assert.Single(_resolver.GetServices(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        [WpfFact]
        public void NonContractRegistrationsShouldResolveCorrectly()
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<AnotherView>(_resolver.GetService<IViewFor<AnotherViewModel>>());
            }
        }

        [WpfFact]
        public void ContractRegistrationsShouldResolveCorrectly()
        {
            using (_resolver.WithResolver())
            {
                Assert.IsType<ContractExampleView>(_resolver.GetService(typeof(IViewFor<ExampleViewModel>), "contract"));
            }
        }

        [Fact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedWhenRequested()
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, NeverUsedView.Instances);
            }
        }

        [WpfFact]
        public void SingleInstanceViewsShouldOnlyBeInstantiatedOnce()
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, SingleInstanceExampleView.Instances);

                var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>));
                Assert.Equal(1, SingleInstanceExampleView.Instances);

                Assert.Same(instance, instance2);
            }
        }

        [WpfFact]
        public void SingleInstanceViewsWithContractShouldResolveCorrectly()
        {
            using (_resolver.WithResolver())
            {
                Assert.Equal(0, SingleInstanceWithContractExampleView.Instances);

                var instance = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                var instance2 = _resolver.GetService(typeof(IViewFor<SingleInstanceExampleViewModel>), "contract");
                Assert.Equal(1, SingleInstanceWithContractExampleView.Instances);

                Assert.Same(instance, instance2);
            }
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
