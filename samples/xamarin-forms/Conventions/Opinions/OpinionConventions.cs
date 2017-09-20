﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Conventional;
using Xunit;

namespace Conventions.Opinions
{
    public class OpinionConventions : IClassFixture<BaseFixture>
    {
        private readonly BaseFixture _baseFixture;

        public OpinionConventions(BaseFixture baseFixture)
        {
            _baseFixture = baseFixture;
        }

        [Fact]
        public void AllPropertiesMustBeImmutable()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.AllPropertiesMustBeImmutable);
        }

        [Fact]
        public void AllPropertiesMustBeInstantiatedDuringConstruction()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.AllPropertiesMustBeAssignedDuringConstruction());
        }

        [Fact]
        public void AsyncMethodsNamesMustEndInAsync()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.AsyncMethodsMustHaveAsyncSuffix);
        }

        [Fact]
        public void ClassesMustOnlyContainToDoOrNoteComments()
        {
            ThisSolution.MustConformTo(Convention.MustOnlyContainToDoAndNoteComments);
        }

        [Fact]
        public void CollectionPropertiesMustBeImmutable()
        {
            _baseFixture.AllAssemblies.MustConformTo(Convention.CollectionPropertiesMustBeImmutable);
        }
    }
}
