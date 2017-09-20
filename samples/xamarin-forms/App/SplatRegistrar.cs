// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Akavache;

using Utility;

using Splat;
using Genesis.Ensure;

namespace App
{
    // ReactiveUI depends on Splat, which is essentially a service locator. Thus, we cannot rely solely on our
    // composition root to supply dependencies throughout the application. We must also prime Splat with the
    // information it requires.
    public abstract class SplatRegistrar
    {
        public void Register(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot)
        {
            Ensure.ArgumentNotNull(splatLocator, nameof(splatLocator));
            Ensure.ArgumentNotNull(compositionRoot, nameof(compositionRoot));

            RegisterViews(splatLocator);
            RegisterScreen(splatLocator, compositionRoot);
            RegisterCommandBinders(splatLocator, compositionRoot);
            RegisterPlatformComponents(splatLocator, compositionRoot);
            InitializeAkavache();
        }

        protected abstract void RegisterPlatformComponents(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot);

        protected abstract void RegisterViews(IMutableDependencyResolver splatLocator);

        private void InitializeAkavache()
        {
            BlobCache.ApplicationName = "ReactiveTwitter";
            BlobCache.EnsureInitialized();
        }

        private void RegisterCommandBinders(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot)
        {
        }

        private void RegisterScreen(IMutableDependencyResolver splatLocator, CompositionRoot compositionRoot)
        {
        }
    }
}
