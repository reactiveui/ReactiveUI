using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;
// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Core;
public class ReflectionTypeEqualityTests
{
    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void AssemblyGetTypes_ContainsTypeWithMatchingAQN()
    {
        // This test verifies that assembly.GetTypes() returns a type with the same AQN as typeof()
        var typeFromTypeof = typeof(FooView);
        var aqnFromTypeof = typeFromTypeof.AssemblyQualifiedName!;

        // Count how many times ReactiveUI.Tests.Core is loaded
        var testAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name == "ReactiveUI.Tests.Core")
            .ToList();
        Console.WriteLine($"ReactiveUI.Tests.Core loaded {testAssemblies.Count} times");
        foreach (var asm in testAssemblies)
        {
            Console.WriteLine($"  - {asm.FullName}, Location: {asm.Location}, HashCode: {asm.GetHashCode()}");
        }

        // Search through all loaded assemblies
        bool found = false;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.AssemblyQualifiedName == aqnFromTypeof)
                    {
                        found = true;
                        Console.WriteLine($"Found type: {t.FullName}");
                        Console.WriteLine($"Assembly from GetTypes: {t.Assembly.FullName}");
                        Console.WriteLine($"Assembly from typeof: {typeFromTypeof.Assembly.FullName}");
                        Console.WriteLine($"Assembly.Equals: {t.Assembly.Equals(typeFromTypeof.Assembly)}");
                        Console.WriteLine($"Assembly ReferenceEquals: {ReferenceEquals(t.Assembly, typeFromTypeof.Assembly)}");
                        Console.WriteLine($"typeof AQN: {aqnFromTypeof}");
                        Console.WriteLine($"assembly.GetTypes() AQN: {t.AssemblyQualifiedName}");
                        Console.WriteLine($"AQNs match: {t.AssemblyQualifiedName == aqnFromTypeof}");
                        Console.WriteLine($"Type.Equals: {t.Equals(typeFromTypeof)}");
                        Console.WriteLine($"Type ==: {t == typeFromTypeof}");
                        Console.WriteLine($"ReferenceEquals: {ReferenceEquals(t, typeFromTypeof)}");
                        Console.WriteLine($"typeof hashcode: {typeFromTypeof.GetHashCode()}");
                        Console.WriteLine($"GetTypes hashcode: {t.GetHashCode()}");
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
            catch
            {
                // Continue
            }
        }

        Assert.That(found, Is.True, "Should find a type with matching AQN via assembly.GetTypes()");
        Assert.Fail("Intentional fail to see console output");
    }

    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void ReallyFindType_ShouldReturn_SameInstanceAs_Typeof_ForSimpleType()
    {
        // Arrange
        var typeFromTypeof = typeof(FooView);
        var assemblyQualifiedName = typeFromTypeof.AssemblyQualifiedName!;

        // Act
        var typeFromReflection = Reflection.ReallyFindType(assemblyQualifiedName, false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(typeFromReflection, Is.Not.Null);
            Assert.That(typeFromReflection, Is.EqualTo(typeFromTypeof), "Types should be equal");
            Assert.That(ReferenceEquals(typeFromReflection, typeFromTypeof), Is.True, "Types should be the same instance");
        }
    }

    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void ReallyFindType_ShouldReturn_SameInstanceAs_Typeof_ForGenericInterfaceType()
    {
        // Arrange
        var typeFromTypeof = typeof(IViewFor<FooViewModel>);
        var assemblyQualifiedName = typeFromTypeof.AssemblyQualifiedName!;

        // Act
        var typeFromReflection = Reflection.ReallyFindType(assemblyQualifiedName, false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(typeFromReflection, Is.Not.Null, "ReallyFindType should find the type");
            Assert.That(typeFromReflection, Is.EqualTo(typeFromTypeof), "Types should be equal");
            Assert.That(ReferenceEquals(typeFromReflection, typeFromTypeof), Is.True, "Types should be the same instance for DI lookup to work");
        }
    }

    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void MakeGenericType_ShouldReturn_SameInstanceAs_Typeof()
    {
        // This tests the hypothesis that MakeGenericType returns the same instance as typeof
        var typeFromTypeof = typeof(IViewFor<FooViewModel>);
        var typeFromMakeGeneric = typeof(IViewFor<>).MakeGenericType(typeof(FooViewModel));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(typeFromMakeGeneric, Is.EqualTo(typeFromTypeof), "Types should be equal");
            Assert.That(ReferenceEquals(typeFromMakeGeneric, typeFromTypeof), Is.True, "MakeGenericType should return the same instance as typeof");
        }
    }

    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void ReallyFindType_ShouldReturn_SameInstanceAs_MakeGenericType()
    {
        // Arrange
        var typeFromMakeGeneric = typeof(IViewFor<>).MakeGenericType(typeof(FooViewModel));
        var assemblyQualifiedName = typeFromMakeGeneric.AssemblyQualifiedName!;

        // Act
        var typeFromReflection = Reflection.ReallyFindType(assemblyQualifiedName, false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(typeFromReflection, Is.Not.Null);
            Assert.That(typeFromReflection, Is.EqualTo(typeFromMakeGeneric), "Types should be equal");
            Assert.That(ReferenceEquals(typeFromReflection, typeFromMakeGeneric), Is.True, "Types should be the same instance");
        }
    }

    [Test]
    [Explicit("Disabled due to NUnit bug with Type reference equality - see https://github.com/nunit/nunit/issues/5092")]
    public void ModernDependencyResolver_ShouldFind_ServiceRegisteredWith_Typeof_WhenLookedUpWith_ReallyFindType()
    {
        // Arrange
        var resolver = new ModernDependencyResolver();
        var serviceType = typeof(IViewFor<FooViewModel>);

        // Register using typeof
        resolver.Register(() => new FooView(), serviceType);

        // Act - lookup using ReallyFindType
        var assemblyQualifiedName = serviceType.AssemblyQualifiedName!;
        var typeFromReflection = Reflection.ReallyFindType(assemblyQualifiedName, false);

        Assert.That(typeFromReflection, Is.Not.Null, "ReallyFindType should find the type");

        var service = resolver.GetService(typeFromReflection!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service, Is.Not.Null, "Should be able to retrieve service using type from ReallyFindType");
            Assert.That(service, Is.TypeOf<FooView>());
        }
    }
}