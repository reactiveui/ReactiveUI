// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace EventBuilder.Core.Reflection.Compilation
{
    /// <summary>
    /// Cache for KnownTypeReferences.
    /// Based on https://github.com/icsharpcode/ILSpy/blob/master/ICSharpCode.Decompiler/TypeSystem/Implementation/KnownTypeCache.cs
    /// and the ILSpy project.
    /// </summary>
    internal sealed class KnownTypeCache
    {
        private readonly ICompilation _compilation;
        private readonly IType[] _knownTypes = new IType[(int)KnownTypeCode.MemoryOfT + 1];

        public KnownTypeCache(ICompilation compilation)
        {
            _compilation = compilation;
        }

        public IType FindType(KnownTypeCode typeCode)
        {
            IType type = LazyInit.VolatileRead(ref _knownTypes[(int)typeCode]);
            if (type != null)
            {
                return type;
            }

            return LazyInit.GetOrSet(ref _knownTypes[(int)typeCode], SearchType(typeCode));
        }

        private IType SearchType(KnownTypeCode typeCode)
        {
            KnownTypeReference typeRef = KnownTypeReference.Get(typeCode);
            if (typeRef == null)
            {
                return SpecialType.UnknownType;
            }

            var typeName = new TopLevelTypeName(typeRef.Namespace, typeRef.Name, typeRef.TypeParameterCount);
            foreach (IModule asm in _compilation.Modules)
            {
                var typeDef = asm.GetTypeDefinition(typeName);
                if (typeDef != null)
                {
                    return typeDef;
                }
            }

            return new UnknownType(typeName);
        }
    }
}
