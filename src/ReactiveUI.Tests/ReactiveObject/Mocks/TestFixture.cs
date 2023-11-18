// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DynamicData.Binding;

namespace ReactiveUI.Tests;

/// <summary>
/// A test fixture.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
[DataContract]
public class TestFixture : ReactiveObject
{
        [IgnoreDataMember]
        [JsonIgnore]
        private string? _isNotNullString;

        [IgnoreDataMember]
        [JsonIgnore]
        private string? _isOnlyOneWord;

        private string? _notSerialized;

        [IgnoreDataMember]
        [JsonIgnore]
        private int? _nullableInt;

        [IgnoreDataMember]
        [JsonIgnore]
        private List<string>? _stackOverflowTrigger;

        [IgnoreDataMember]
        [JsonIgnore]
        private string? _usesExprRaiseSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        public TestFixture() => TestCollection = new ObservableCollectionExtended<int>();

        /// <summary>
        /// Gets or sets the is not null string.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public string? IsNotNullString
        {
            get => _isNotNullString;
            set => this.RaiseAndSetIfChanged(ref _isNotNullString, value);
        }

        /// <summary>
        /// Gets or sets the is only one word.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public string? IsOnlyOneWord
        {
            get => _isOnlyOneWord;
            set => this.RaiseAndSetIfChanged(ref _isOnlyOneWord, value);
        }

        /// <summary>
        /// Gets or sets the not serialized.
        /// </summary>
        public string? NotSerialized
        {
            get => _notSerialized;
            set => this.RaiseAndSetIfChanged(ref _notSerialized, value);
        }

        /// <summary>
        /// Gets or sets the nullable int.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public int? NullableInt
        {
            get => _nullableInt;
            set => this.RaiseAndSetIfChanged(ref _nullableInt, value);
        }

        /// <summary>
        /// Gets or sets the poco property.
        /// </summary>
        [DataMember]
        [field: IgnoreDataMember]
        [JsonRequired]
        public string? PocoProperty { get; set; }

        /// <summary>
        /// Gets or sets the stack overflow trigger.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public List<string>? StackOverflowTrigger
        {
            get => _stackOverflowTrigger;
            set => this.RaiseAndSetIfChanged(ref _stackOverflowTrigger, value?.ToList());
        }

        /// <summary>
        /// Gets or sets the test collection.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public ObservableCollectionExtended<int> TestCollection { get; set; }

        /// <summary>
        /// Gets or sets the uses expr raise set.
        /// </summary>
        [DataMember]
        [JsonRequired]
        public string? UsesExprRaiseSet
        {
            get => _usesExprRaiseSet;
            set => this.RaiseAndSetIfChanged(ref _usesExprRaiseSet, value);
        }
}
