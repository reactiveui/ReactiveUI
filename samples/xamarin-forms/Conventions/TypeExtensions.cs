using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Conventions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(this Assembly assembly, Type openGenericType)
        {
            return from type in assembly.GetExportedTypes()
                   from z in type.GetInterfaces()
                   let y = type.BaseType
                   where
                       (y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                       (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select type;
        }
    }
}