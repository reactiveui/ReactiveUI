// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DynamicData.Binding;

namespace ReactiveUI.Tests
{
    [DataContract]
    public class TestFixture : ReactiveObject
    {
        [IgnoreDataMember]
        private string? _isNotNullString;

        [IgnoreDataMember]
        private string? _isOnlyOneWord;

        private string? _notSerialized;

        [IgnoreDataMember]
        private int? _nullableInt;

        [IgnoreDataMember]
        private string? _pocoProperty;

        [IgnoreDataMember]
        private List<string>? _stackOverflowTrigger;

        [IgnoreDataMember]
        private string? _usesExprRaiseSet;

        public TestFixture() => TestCollection = new ObservableCollectionExtended<int>();

        [DataMember]
        public string? IsNotNullString
        {
            get => _isNotNullString;
            set => this.RaiseAndSetIfChanged(ref _isNotNullString, value);
        }

        [DataMember]
        public string? IsOnlyOneWord
        {
            get => _isOnlyOneWord;
            set => this.RaiseAndSetIfChanged(ref _isOnlyOneWord, value);
        }

        public string? NotSerialized
        {
            get => _notSerialized;
            set => this.RaiseAndSetIfChanged(ref _notSerialized, value);
        }

        [DataMember]
        public int? NullableInt
        {
            get => _nullableInt;
            set => this.RaiseAndSetIfChanged(ref _nullableInt, value);
        }

        [DataMember]
        public string? PocoProperty
        {
            get => _pocoProperty;
            set => _pocoProperty = value;
        }

        [DataMember]
        public List<string>? StackOverflowTrigger
        {
            get => _stackOverflowTrigger;
            set => this.RaiseAndSetIfChanged(ref _stackOverflowTrigger, value?.ToList());
        }

        [DataMember]
        public ObservableCollectionExtended<int> TestCollection { get; protected set; }

        [DataMember]
        public string? UsesExprRaiseSet
        {
            get => _usesExprRaiseSet;
            set => this.RaiseAndSetIfChanged(ref _usesExprRaiseSet, value);
        }
    }
}
