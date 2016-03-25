using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	class DataAnnotationsCatalog
			: ComposablePartCatalog
			, ICompositionElement
			, INotifyComposablePartCatalogChanged
	{
		private volatile bool _isDisposed;
		private readonly string _scopeFull;
		private readonly object _lock = new object();

		private ComposablePartCatalog _rootCatalog;
		private List<ComposablePartDefinition> _exports;

		#region Implementation of ICompositionElement

		public string DisplayName
		{
			get
			{
				return _scopeFull;
			}
		}

		public ICompositionElement Origin
		{
			get { return _rootCatalog as ICompositionElement; }
		}

		#endregion

		public DataAnnotationsCatalog(ComposablePartCatalog rootCatalog, string scopeFull)
		{
			_scopeFull = scopeFull;
			_rootCatalog = rootCatalog;

			var notifyCatalog = _rootCatalog as INotifyComposablePartCatalogChanged;
			if (notifyCatalog != null)
			{
				notifyCatalog.Changed += OnChangedInternal;
				notifyCatalog.Changing += OnChangingInternal;
			}
		}

		private bool ScopeEquals(string scope)
		{
			var ret = (string.IsNullOrEmpty(scope) && string.IsNullOrEmpty(_scopeFull)) || string.Equals(scope, _scopeFull);
			return ret;
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
						var ret = s.ContractName == item.ContractName && identityMetadataName1 == identityMetadataName2;
						return ret;
					}))
				{
					throw new CompositionException($"The definition cannot have a duplicate Contact name and Type identity in different scopes. {definition}");
				}
			}
		}

		private bool Filter(ComposablePartDefinition definition)
		{
			var definitionWasChecked = false;
			foreach (var item in definition.GetExportMetadataValueWithKey("ScopeFullName"))
			{
				if (!definitionWasChecked)
				{
					CheckDefinition(definition);
					definitionWasChecked = true;
				}

				var array = item as string[];
				if (array != null)
				{
					if (array.Any(ScopeEquals))
					{
						return true;
					}
				}
				else
				{
					var scope = item as string;
					if (ScopeEquals(scope))
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
			EventHandler<ComposablePartCatalogChangeEventArgs> changedEvent = Changed;
			changedEvent?.Invoke(this, e);
		}

		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changingEvent = Changing;
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