using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
    sealed class CompositionScopeManager
        : ICompositionScopeManager
    {
        private readonly ComposablePartCatalog _customCatalog;
        private readonly ICompositionScopeManager _parentManager;
        private readonly Lazy<ExportProvider> _lazyExportProvider;
        private readonly IComposablePartCatalogFactory _catalogFactory;
        private readonly Lazy<ComposablePartCatalog> _lazyComposablePartCatalog;

        public string ScopeName { get; }

        public string ScopeFullName
        {
            get
            {
                var parentFullName = ExportProvider?.GetExportedValue<ICompositionScopeManager>().ScopeFullName;
                if (string.IsNullOrEmpty(parentFullName))
                {
                    return ScopeName;
                }

                var ret = parentFullName + "/" + ScopeName;
                return ret;
            }
        }

        public ExportProvider ExportProvider => _lazyExportProvider.Value;

        public ComposablePartCatalog Catalog => _lazyComposablePartCatalog.Value;

        public CompositionScopeManager(
            IComposablePartCatalogFactory catalogFactory)
            : this(null, null)
        {
            _catalogFactory = catalogFactory;
        }

        public CompositionScopeManager(
            string scope,
            CompositionScopeManager parentManager)
            : this(scope, null, parentManager)
        {
        }

        public CompositionScopeManager(
            string scope,
            ComposablePartCatalog catalog,
            CompositionScopeManager parentManager)
        {
            ScopeName = scope;
            _customCatalog = catalog;
            _parentManager = parentManager;
            _lazyExportProvider = new Lazy<ExportProvider>(ExportProviderFactory);
            _lazyComposablePartCatalog = new Lazy<ComposablePartCatalog>(ComposablePartCatalogFactory);

            if (_parentManager != null)
            {
                _catalogFactory = parentManager._catalogFactory;
            }
        }

        private static void CheckScopeName(string scope)
        {
            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentException("The Scope name cannot be null or empty", nameof(scope));
            }

            if (scope.IndexOf('/') != -1)
            {
                throw new ArgumentException("The Scope name cannot contain '/'", nameof(scope));
            }
        }

        private ExportProvider ExportProviderFactory()
        {
            var ret = _parentManager.ExportProvider == null
                ? new CompositionContainer(Catalog, CompositionOptions.DisableSilentRejection)
                : new CompositionContainer(Catalog, CompositionOptions.DisableSilentRejection, _parentManager.ExportProvider);

            var batch = new CompositionBatch();
            batch.AddExportedValue<ICompositionScopeManager>(this);
            ret.Compose(batch);

            return ret;
        }

        private ComposablePartCatalog ComposablePartCatalogFactory()
        {
            if (_customCatalog != null)
            {
                return _customCatalog;
            }

            var ret = _catalogFactory.GetComposablePartCatalog(ScopeFullName);
            return ret;
        }

        public ICompositionScopeManager CreateCompositionScopeManager(string scope)
        {
            CheckScopeName(scope);

            var ret = new CompositionScopeManager(scope, this);
            return ret;
        }

        public ICompositionScopeManager CreateCompositionScopeManager(string scope, ComposablePartCatalog catalog)
        {
            CheckScopeName(scope);

            var ret = new CompositionScopeManager(scope, catalog, this);
            return ret;
        }
    }
}
