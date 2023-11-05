// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing.Tests
{
    /// <summary>
    /// An <see cref="IBuilder"/> that constructs a <see cref="TestFixture"/>.
    /// </summary>
    public class TestFixtureBuilder : IBuilder
    {
        private int _count;
        private string? _name;
        private List<string>? _tests = new();
        private Dictionary<string, string> _variables = new();

        /// <summary>
        /// Performs an implicit conversion from <see cref="TestFixtureBuilder"/> to <see cref="TestFixture"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The test fixture.</returns>
        public static implicit operator TestFixture(TestFixtureBuilder builder) => ToTestFixture(builder);

        /// <summary>
        /// Performs conversion from <see cref="TestFixtureBuilder"/> to <see cref="TestFixture"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The test fixture.</returns>
        public static TestFixture ToTestFixture(TestFixtureBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Build();
        }

        /// <summary>
        /// Adds the count to the builder.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithCount(int count) => this.With(out _count, count);

        /// <summary>
        /// Adds the dictionary to the builder.
        /// </summary>
        /// <param name="variables">The dictionary.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithDictionary(Dictionary<string, string> variables) => this.With(ref _variables, variables);

        /// <summary>
        /// Adds the key value pair to the builder.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithKeyValue(KeyValuePair<string, string> keyValuePair) => this.With(ref _variables, keyValuePair);

        /// <summary>
        /// Adds a key value pair to the builder.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithKeyValue(string key, string value) => this.With(ref _variables, key, value);

        /// <summary>
        /// Adds a name to the builder.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithName(string name) => this.With(out _name, name);

        /// <summary>
        /// Adds a test to the builder.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithTest(string test) => this.With(ref _tests, test);

        /// <summary>
        /// Adds tests to the builder.
        /// </summary>
        /// <param name="tests">The tests.</param>
        /// <returns>The builder.</returns>
        public TestFixtureBuilder WithTests(IEnumerable<string> tests) => this.With(ref _tests, tests);

        private TestFixture Build() => new()
        {
            Name = _name,
            Count = _count,
            Tests = _tests,
            Variables = _variables
        };
    }
}
