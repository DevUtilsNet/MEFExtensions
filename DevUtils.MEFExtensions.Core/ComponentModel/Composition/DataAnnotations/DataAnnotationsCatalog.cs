using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	class DataAnnotationsCatalog
			: ComposablePartCatalog
			, ICompositionElement
			, INotifyComposablePartCatalogChanged
	{
		private volatile bool _isDisposed;
		private readonly ScopeName _scopeName;
		private readonly object _lock = new object();

		private ComposablePartCatalog _rootCatalog;
		private List<ComposablePartDefinition> _exports;

		#region Implementation of ICompositionElement

		public string DisplayName
		{
			get
			{
				var ret = _scopeName.FullName;
				return ret;
			}
		}

		public ICompositionElement Origin => _rootCatalog as ICompositionElement;

		#endregion

		public DataAnnotationsCatalog(ComposablePartCatalog rootCatalog, ScopeName scopeName)
		{
			_scopeName = scopeName;
			_rootCatalog = rootCatalog;

			var notifyCatalog = _rootCatalog as INotifyComposablePartCatalogChanged;
			if (notifyCatalog != null)
			{
				notifyCatalog.Changed += OnChangedInternal;
				notifyCatalog.Changing += OnChangingInternal;
			}
		}

		private static void CheckDefinition(ComposablePartDefinition definition)
		{
			foreach (var item in definition.ExportDefinitions)
			{
				object identityMetadataName1;
				item.Metadata.TryGetValue(CompositionConstants.ExportTypeIdentityMetadataName, out identityMetadataName1);

				if (definition.ExportDefinitions.Where(w => w != item).Any(
					s =>
					{
						object identityMetadataName2;
						s.Metadata.TryGetValue(CompositionConstants.ExportTypeIdentityMetadataName, out identityMetadataName2);
						var ret = identityMetadataName1 == identityMetadataName2 //&& s.ContractName == item.ContractName
						;
						return ret;
					}))
				{
					throw new CompositionException($"Duplicate Type identity in different scopes are not supported. {definition}");
				}
			}
		}

		private bool Filter(ComposablePartDefinition definition)
		{
			var definitionWasChecked = false;

			foreach (var item in definition.GetExportMetadataValueWithKey(nameof(ScopedExportAttribute.ScopeFullName)))
			{
				if (!definitionWasChecked)
				{
					CheckDefinition(definition);
					definitionWasChecked = true;
				}

				var array = item as string[];
				if (array != null)
				{
					if (array.Select(s => new ScopeName(s)).Contains(_scopeName))
					{
						return true;
					}
				}
				else
				{
					var scope = new ScopeName(item as string);
					if (scope == _scopeName)
					{
						return true;
					}
				}
			}

			return false;
		}

		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			ThrowIfDisposed();

			if (_exports == null)
			{
				lock (_lock)
				{
					if (_exports == null)
					{
						_exports = new List<ComposablePartDefinition>();

						foreach (var item in _rootCatalog.Where(Filter))
						{
							_exports.Add(item);
						}
					}
				}
			}

			return _exports.GetEnumerator();
		}

		#region Implementation of INotifyComposablePartCatalogChanged

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		#endregion

		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			var changedEvent = Changed;
			changedEvent?.Invoke(this, e);
		}

		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			var changingEvent = Changing;
			changingEvent?.Invoke(this, e);
		}

		private void OnChangedInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			var processedArgs = ProcessEventArgs(e);
			if (processedArgs != null)
			{
				OnChanged(ProcessEventArgs(processedArgs));
			}
		}

		private void OnChangingInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			_exports = null;
			var processedArgs = ProcessEventArgs(e);
			if (processedArgs != null)
			{
				OnChanging(ProcessEventArgs(processedArgs));
			}
		}

		private ComposablePartCatalogChangeEventArgs ProcessEventArgs(ComposablePartCatalogChangeEventArgs e)
		{
			var result = new ComposablePartCatalogChangeEventArgs(e.AddedDefinitions.Where(Filter), e.RemovedDefinitions.Where(Filter), e.AtomicComposition);

			if (result.AddedDefinitions.Any() || result.RemovedDefinitions.Any())
			{
				return result;
			}

			return null;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!disposing)
				{
					return;
				}

				if (!_isDisposed)
				{
					INotifyComposablePartCatalogChanged notifyCatalog = null;
					try
					{
						lock (_lock)
						{
							if (!_isDisposed)
							{
								_isDisposed = true;
								notifyCatalog = _rootCatalog as INotifyComposablePartCatalogChanged;
								_rootCatalog = null;
							}
						}
					}
					finally
					{
						if (notifyCatalog != null)
						{
							notifyCatalog.Changed -= OnChangedInternal;
							notifyCatalog.Changing -= OnChangingInternal;
						}
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}
	}
}