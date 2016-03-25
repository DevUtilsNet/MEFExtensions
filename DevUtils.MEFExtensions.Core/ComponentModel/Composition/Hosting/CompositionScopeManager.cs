using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	sealed class CompositionScopeManager
			: ICompositionScopeManager
	{
		private readonly ComposablePartCatalog _customCatalog;
		private readonly ICompositionScopeManager _parentManager;
		private readonly Lazy<CompositionContainer> _lazyContainer;
		private readonly IComposablePartCatalogFactory _catalogFactory;
		private readonly Lazy<ComposablePartCatalog> _lazyComposablePartCatalog;

		private CompositionScopeManagerDisposeGuard _disposeGuard;

		public string ScopeName { get; }

		public string ScopeFullName
		{
			get
			{
				var parentFullName = _parentManager?.Container.GetExportedValue<ICompositionScopeManager>().ScopeFullName;
				if (string.IsNullOrEmpty(parentFullName))
				{
					return ScopeName;
				}

				var ret = parentFullName + "/" + ScopeName;
				return ret;
			}
		}

		public CompositionContainer Container => _lazyContainer.Value;

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
			_lazyContainer = new Lazy<CompositionContainer>(ContainerFactory);
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

		private CompositionContainer ContainerFactory()
		{
			var catalog = new AggregateCatalog(Catalog, new TypeCatalog(typeof(CompositionScopeManagerDisposeGuard)));

			CompositionContainer ret;
			if (_parentManager == null)
			{
				ret = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
			}
			else
			{
				ret = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection, _parentManager.Container);

				_disposeGuard = _parentManager.Container.GetExportedValue<CompositionScopeManagerDisposeGuard>();
				_disposeGuard.Add(this);
			}

			var batch = new CompositionBatch();
			batch.AddExportedValue<ICompositionScopeManager>(this);
			ret.Compose(batch);

			return ret;
		}

		private ComposablePartCatalog ComposablePartCatalogFactory()
		{
			var ret = _customCatalog ?? _catalogFactory.GetComposablePartCatalog(ScopeFullName);
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

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_disposeGuard?.Remove(this);

			if (_lazyContainer.IsValueCreated)
			{
				_lazyContainer.Value.Dispose();
			}
		}

		#endregion
	}
}
