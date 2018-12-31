using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace EventBuilder.NuGet
{
    /// <summary>
    /// Based off Dave Glick's articles here: https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-3
    /// Creates and caches SourceRepository objects, which are
    /// the combination of PackageSource instances with a set
    /// of supported resource providers. It also manages the set
    /// of default source repositories.
    /// </summary>
    internal class SourceRepositoryProvider : ISourceRepositoryProvider
    {
        private static readonly string[] DefaultSources =
        {
            "https://api.nuget.org/v3/index.json"
        };

        private readonly List<SourceRepository> _defaultRepositories = new List<SourceRepository>();

        private readonly ConcurrentDictionary<PackageSource, SourceRepository> _repositoryCache
            = new ConcurrentDictionary<PackageSource, SourceRepository>();

        private readonly List<Lazy<INuGetResourceProvider>> _resourceProviders;

        public SourceRepositoryProvider(ISettings settings)
        {
            // Create the package source provider (needed primarily to get default sources)
            PackageSourceProvider = new PackageSourceProvider(settings);

            // Add the v3 provider as default
            _resourceProviders = new List<Lazy<INuGetResourceProvider>>();
            _resourceProviders.AddRange(Repository.Provider.GetCoreV3());

            AddGlobalDefaults();
            AddDefaultPackageSources();
        }

        /// <inheritdoc />
        public IPackageSourceProvider PackageSourceProvider { get; }

        /// <summary>
        /// Add the global sources to the default repositories.
        /// </summary>
        public void AddGlobalDefaults()
        {
            _defaultRepositories.AddRange(PackageSourceProvider.LoadPackageSources()
                .Where(x => x.IsEnabled)
                .Select(x => new SourceRepository(x, _resourceProviders)));
        }

        public void AddDefaultPackageSources()
        {
            foreach (string defaultSource in DefaultSources)
            {
                AddDefaultRepository(defaultSource);
            }
        }

        /// <summary>
        /// Adds a default source repository to the front of the list.
        /// </summary>
        public void AddDefaultRepository(string packageSource) => _defaultRepositories.Insert(0, CreateRepository(packageSource));

        public IEnumerable<SourceRepository> GetDefaultRepositories() => _defaultRepositories;

        /// <summary>
        /// Creates or gets a non-default source repository.
        /// </summary>
        public SourceRepository CreateRepository(string packageSource) => CreateRepository(new PackageSource(packageSource), FeedType.Undefined);

        /// <summary>
        /// Creates or gets a non-default source repository by PackageSource.
        /// </summary>
        public SourceRepository CreateRepository(PackageSource packageSource) => CreateRepository(packageSource, FeedType.Undefined);

        /// <summary>
        /// Creates or gets a non-default source repository by PackageSource.
        /// </summary>
        public SourceRepository CreateRepository(PackageSource packageSource, FeedType feedType) =>
            _repositoryCache.GetOrAdd(packageSource, x => new SourceRepository(packageSource, _resourceProviders));

        /// <summary>
        /// Gets all cached repositories.
        /// </summary>
        public IEnumerable<SourceRepository> GetRepositories() => _repositoryCache.Values;
    }
}
