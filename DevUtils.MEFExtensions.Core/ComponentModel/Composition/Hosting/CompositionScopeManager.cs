﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
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

		public string ScopeFullName
		{
			get
			{
				var parentFullName = ParentManager?.Container.GetExportedValue<ICompositionScopeManager>().ScopeFullName;
				if (string.IsNullOrEmpty(parentFullName))
				{
					return ScopeName;
				}

				var ret = parentFullName + "/" + ScopeName;
				return ret;
			}
		}

		public ICompositionScopeManager ParentManager { get; set; }

		public CompositionContainer Container => _lazyContainer.Value;

		public ComposablePartCatalog Catalog => _lazyComposablePartCatalog.Value;

		public CompositionScopeManager(
			IComposablePartCatalogFactory catalogFactory)
			: this(null, false, null)
		{
			_catalogFactory = catalogFactory;
		}

		public CompositionScopeManager(
			string scope,
			IComposablePartCatalogFactory catalogFactory)
			: this(scope, false, null)
		{
			_catalogFactory = catalogFactory;
		}

		public CompositionScopeManager(
			string scope, 
			bool cascadeDelete, 
			CompositionScopeManager parentManager)
			: this(scope, cascadeDelete, null, parentManager)
		{
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

		public static string CheckScopeName(string scope)
		{
			if (string.IsNullOrEmpty(scope))
			{
				throw new ArgumentException("The Scope name cannot be null or empty", nameof(scope));
			}

			if (scope.IndexOf('/') != -1)
			{
				throw new ArgumentException("The Scope name cannot contain '/'", nameof(scope));
			}

			return scope;
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

		public ICompositionScopeManager CreateCompositionScopeManager(string scope)
		{
			var ret = CreateCompositionScopeManager(scope, true);
			return ret;
		}

		public ICompositionScopeManager CreateCompositionScopeManager(string scope, bool cascadeDelete)
		{
			CheckScopeName(scope);

			var ret = new CompositionScopeManager(scope, cascadeDelete, this);
			return ret;
		}

		public ICompositionScopeManager CreateCompositionScopeManager(string scope, ComposablePartCatalog customCatalog)
		{
			var ret = CreateCompositionScopeManager(scope, true, customCatalog);
			return ret;
		}

		public ICompositionScopeManager CreateCompositionScopeManager(string scope, bool cascadeDelete, ComposablePartCatalog customCatalog)
		{
			CheckScopeName(scope);

			var ret = new CompositionScopeManager(scope, cascadeDelete, customCatalog, this);
			return ret;
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
