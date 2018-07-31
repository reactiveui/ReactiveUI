// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ReactiveUI.Fody
{
    public static class CecilExtensions
    {
        public static void Emit(this MethodBody body, Action<ILProcessor> il)
        {
            il(body.GetILProcessor());
        }

        public static GenericInstanceMethod MakeGenericMethod(this MethodReference method, params TypeReference[] genericArguments)
        {
            var result = new GenericInstanceMethod(method);
            foreach (var argument in genericArguments)
                result.GenericArguments.Add(argument);
            return result;
        }

        public static bool IsAssignableFrom(this TypeReference baseType, TypeReference type, Action<string> logger = null)
        {
            return baseType.Resolve().IsAssignableFrom(type.Resolve(), logger);
        }

        public static bool IsAssignableFrom(this TypeDefinition baseType, TypeDefinition type, Action<string> logger = null)
        {
            logger = logger ?? (x => {});

            Queue<TypeDefinition> queue = new Queue<TypeDefinition>();
            queue.Enqueue(type);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                logger(current.FullName);

                if (baseType.FullName == current.FullName)
                    return true;

                if (current.BaseType != null)
                    queue.Enqueue(current.BaseType.Resolve());

                foreach (var @interface in current.Interfaces)
                {
                    queue.Enqueue(@interface.InterfaceType.Resolve());
                }
            }

            return false;
        }

        public static bool IsDefined(this IMemberDefinition member, TypeReference attributeType)
        {
            return member.HasCustomAttributes && member.CustomAttributes.Any(x => x.AttributeType.FullName == attributeType.FullName);
        }

        public static MethodReference Bind(this MethodReference method, GenericInstanceType genericType)
        {
            var reference = new MethodReference(method.Name, method.ReturnType, genericType);
            reference.HasThis = method.HasThis;
            reference.ExplicitThis = method.ExplicitThis;
            reference.CallingConvention = method.CallingConvention;

            foreach (var parameter in method.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            return reference;
        }

        /*
        public static MethodReference BindDefinition(this MethodReference method, TypeReference genericTypeDefinition)
        {
            if (!genericTypeDefinition.HasGenericParameters)
                return method;

            var genericDeclaration = new GenericInstanceType(genericTypeDefinition);
            foreach (var parameter in genericTypeDefinition.GenericParameters)
            {
                genericDeclaration.GenericArguments.Add(parameter);
            }
            var reference = new MethodReference(method.Name, method.ReturnType, genericDeclaration);
            reference.HasThis = method.HasThis;
            reference.ExplicitThis = method.ExplicitThis;
            reference.CallingConvention = method.CallingConvention;

            foreach (var parameter in method.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            return reference;
        }
        */

        public static FieldReference BindDefinition(this FieldReference field, TypeReference genericTypeDefinition)
        {
            if (!genericTypeDefinition.HasGenericParameters)
                return field;

            var genericDeclaration = new GenericInstanceType(genericTypeDefinition);
            foreach (var parameter in genericTypeDefinition.GenericParameters)
            {
                genericDeclaration.GenericArguments.Add(parameter);
            }
            var reference = new FieldReference(field.Name, field.FieldType, genericDeclaration);
            return reference;
        }

        public static AssemblyNameReference FindAssembly(this ModuleDefinition currentModule, string assemblyName)
        {
            return currentModule.AssemblyReferences.SingleOrDefault(x => x.Name == assemblyName);
        }

        public static TypeReference FindType(this ModuleDefinition currentModule, string @namespace, string typeName, IMetadataScope scope = null, params string[] typeParameters)
        {
            var result = new TypeReference(@namespace, typeName, currentModule, scope);
            foreach (var typeParameter in typeParameters)
            {
                result.GenericParameters.Add(new GenericParameter(typeParameter, result));
            }
            return result;
        }

        public static bool CompareTo(this TypeReference type, TypeReference compareTo)
        {
            return type.FullName == compareTo.FullName;
        }

/*
        public static IEnumerable<TypeDefinition> GetAllTypes(this ModuleDefinition module)
        {
            var stack = new Stack<TypeDefinition>();
            foreach (var type in module.Types)
            {
                stack.Push(type);
            }
            while (stack.Any())
            {
                var current = stack.Pop();
                yield return current;

                foreach (var nestedType in current.NestedTypes)
                {
                    stack.Push(nestedType);
                }
            }
        }
*/
    }
}
