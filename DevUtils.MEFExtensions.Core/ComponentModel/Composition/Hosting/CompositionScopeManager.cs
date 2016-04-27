using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	sealed class CompositionScopeManager
		: ICompositionScopeManager
	{
		private readonly bool _cascadeDelete;
		private readonly ComposablePartCatalog _customCatalog;
		private readonly Lazy<CompositionContainer> _lazyContainer;
		private readonly IComposablePartCatalogFactory _catalogFactory;
		private readonly Lazy<ComposablePartCatalog> _lazyComposablePartCatalog;

		private CompositionScopeManagerDisposeGuard _disposeGuard;

		public string ScopeName { get; }

		public ScopeName ScopeFullName
		{
			get
			{
				var parentFullName = ParentManager?.Container.GetExportedValue<ICompositionScopeManager>().ScopeFullName;
				if (parentFullName.HasValue)
				{
					var ret = parentFullName.Value / ScopeName;
					return ret;
				}
				else
				{
					var ret = new ScopeName(ScopeName);
					return ret;
				}
			}
		}

		public ICompositionScopeManager ParentManager { get; set; }

		public CompositionContainer Container => _lazyContainer.Value;

		public ComposablePartCatalog Catalog => _lazyComposablePartCatalog.Value;

		public CompositionScopeManager(
			IComposablePartCatalogFactory catalogFactory)
			: this(null, catalogFactory)
		{
		}

		public CompositionScopeManager(
			string scope,
			IComposablePartCatalogFactory catalogFactory)
			: this(scope, false, null,  null)
		{
			_catalogFactory = catalogFactory;
		}

		public CompositionScopeManager(
			string scope,
			bool cascadeDelete,
			ComposablePartCatalog customCatalog,
			CompositionScopeManager parentManager)
		{
			ScopeName = scope;
			_cascadeDelete = cascadeDelete;
			_customCatalog = customCatalog;
			ParentManager = parentManager;
			_lazyContainer = new Lazy<CompositionContainer>(ContainerFactory);
			_lazyComposablePartCatalog = new Lazy<ComposablePartCatalog>(ComposablePartCatalogFactory);

			if (ParentManager != null)
			{
				_catalogFactory = parentManager._catalogFactory;
			}
		}

		private static void CheckScopeName(string scopeName)
		{
			Primitives.ScopeName.CheckSingleScopeName(scopeName);

			if (scopeName == "*" || scopeName == "**")
			{
				throw new ArgumentException("The Scope name cannot be '*' or '**'", nameof(scopeName));
			}
		}

		private CompositionContainer ContainerFactory()
		{
			var catalog = new AggregateCatalog(Catalog, new TypeCatalog(typeof (CompositionScopeManagerDisposeGuard)));

			CompositionContainer ret;
			if (ParentManager == null)
			{
				ret = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
			}
			else
			{
				ret = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection, ParentManager.Container);

				if (_cascadeDelete)
				{
					_disposeGuard = ParentManager.Container.GetExportedValue<CompositionScopeManagerDisposeGuard>();
					_disposeGuard.Add(this);
				}
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

		public ICompositionScopeManager CreateCompositionScopeManager(string scope, ComposablePartCatalog customCatalog = null, bool cascadeDelete = true, bool initializeModules = true)
		{
			CheckScopeName(scope);

			var ret = new CompositionScopeManager(scope, cascadeDelete, customCatalog, this);

			if (initializeModules)
			{
				ret.InitializeModules();
			}
			return ret;
		}

		public void InitializeModules()
		{
			Container.InitializeModules(ScopeFullName);
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			_disposeGuard?.Remove(this);

			if (_lazyContainer.IsValueCreated)
			{
				_lazyContainer.Value.Dispose();
			}

			ParentManager = null;
		}

		#endregion
	}
}
