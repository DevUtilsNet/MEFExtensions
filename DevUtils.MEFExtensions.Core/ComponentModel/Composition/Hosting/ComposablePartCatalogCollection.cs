using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DevUtils.MEFExtensions.Core.Collections.Generic.Extensions;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	sealed class ComposablePartCatalogCollection 
		: ICollection<ComposablePartCatalog>
		, INotifyComposablePartCatalogChanged, IDisposable
	{
		private bool _hasChanged;
		private volatile bool _isDisposed;
		private volatile bool _isCopyNeeded;
		private List<ComposablePartCatalog> _catalogs;

		private readonly object _lock = new object();
		private readonly Action<ComposablePartCatalogChangeEventArgs> _onChanged;
		private readonly Action<ComposablePartCatalogChangeEventArgs> _onChanging;

		public ComposablePartCatalogCollection(IEnumerable<ComposablePartCatalog> catalogs)
				: this(catalogs, null, null)
		{
		}

		public ComposablePartCatalogCollection(
				IEnumerable<ComposablePartCatalog> catalogs,
				Action<ComposablePartCatalogChangeEventArgs> onChanged,
				Action<ComposablePartCatalogChangeEventArgs> onChanging)
		{
			catalogs = catalogs ?? Enumerable.Empty<ComposablePartCatalog>();
			_catalogs = new List<ComposablePartCatalog>(catalogs);
			_onChanged = onChanged;
			_onChanging = onChanging;

			SubscribeToCatalogNotifications(_catalogs);
		}

		public int Count
		{
			get
			{
				ThrowIfDisposed();

				lock (_lock)
				{
					return _catalogs.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get
			{
				ThrowIfDisposed();

				return false;
			}
		}

		internal bool HasChanged
		{
			get
			{
				ThrowIfDisposed();

				lock (_lock)
				{
					return _hasChanged;
				}
			}
		}

		public void Add(ComposablePartCatalog item)
		{
			ThrowIfDisposed();

			var addedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.Parts.ToArray(), false);

			using (var atomicComposition = new AtomicComposition())
			{
				RaiseChangingEvent(addedParts, null, atomicComposition);

				lock (_lock)
				{
					if (_isCopyNeeded)
					{
						_catalogs = new List<ComposablePartCatalog>(_catalogs);
						_isCopyNeeded = false;
					}
					_hasChanged = true;
					_catalogs.Add(item);
				}

				SubscribeToCatalogNotifications(item);

				// Complete after the catalog changes are written
				atomicComposition.Complete();
			}

			RaiseChangedEvent(addedParts, null);
		}

		/// <summary>
		/// Notify when the contents of the Catalog has changed.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>
		/// Notify when the contents of the Catalog has changing.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		public void Clear()
		{
			ThrowIfDisposed();

			// No action is required if we are already empty
			ComposablePartCatalog[] catalogs;
			lock (_lock)
			{
				if (_catalogs.Count == 0)
				{
					return;
				}
				catalogs = _catalogs.ToArray();
			}

			// TODO-MT: This is pretty suspect - we can easily eliminate catalogs that aren't listed as being
			// removed.  Then again, the idea of trying to mutate the catalog on two threads at the same time is pretty
			// suspect to begin with.  When would that ever result in a meaningful composition?

			// We are doing this outside of the lock, so it's possible that the catalog will continute propagating events from things
			// we are about to unsubscribe from. Given the non-specificity of our event, in the worst case scenario we would simply fire 
			// unnecessary events.

			var removedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => catalogs.SelectMany(catalog => catalog.Parts).ToArray(), false);

			// Validate the changes before applying them
			using (var atomicComposition = new AtomicComposition())
			{
				RaiseChangingEvent(null, removedParts, atomicComposition);
				UnsubscribeFromCatalogNotifications(catalogs);

				lock (_lock)
				{
					_catalogs = new List<ComposablePartCatalog>();

					_isCopyNeeded = false;
					_hasChanged = true;
				}

				// Complete after the catalog changes are written
				atomicComposition.Complete();
			}

			RaiseChangedEvent(null, removedParts);
		}

		public bool Contains(ComposablePartCatalog item)
		{
			ThrowIfDisposed();

			lock (_lock)
			{
				return _catalogs.Contains(item);
			}
		}

		public void CopyTo(ComposablePartCatalog[] array, int arrayIndex)
		{
			ThrowIfDisposed();

			lock (_lock)
			{
				_catalogs.CopyTo(array, arrayIndex);
			}
		}

		public bool Remove(ComposablePartCatalog item)
		{
			ThrowIfDisposed();

			lock (_lock)
			{
				if (!_catalogs.Contains(item))
				{
					return false;
				}
			}

			bool isSuccessfulRemoval;

			var removedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.Parts.ToArray(), false);
			using (var atomicComposition = new AtomicComposition())
			{
				RaiseChangingEvent(null, removedParts, atomicComposition);

				lock (_lock)
				{
					if (_isCopyNeeded)
					{
						_catalogs = new List<ComposablePartCatalog>(_catalogs);
						_isCopyNeeded = false;
					}

					isSuccessfulRemoval = _catalogs.Remove(item);
					if (isSuccessfulRemoval)
					{
						_hasChanged = true;
					}
				}

				UnsubscribeFromCatalogNotifications(item);

				// Complete after the catalog changes are written
				atomicComposition.Complete();
			}

			RaiseChangedEvent(null, removedParts);

			return isSuccessfulRemoval;
		}

		public IEnumerator<ComposablePartCatalog> GetEnumerator()
		{
			ThrowIfDisposed();

			lock (_lock)
			{
				IEnumerator<ComposablePartCatalog> enumerator = _catalogs.GetEnumerator();
				_isCopyNeeded = true;
				return enumerator;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!_isDisposed)
				{
					IEnumerable<ComposablePartCatalog> catalogs = null;

					lock (_lock)
					{
						try
						{
							if (!_isDisposed)
							{
								catalogs = _catalogs;
								_catalogs = null;

								_isDisposed = true;
							}
						}
						finally
						{
							if (catalogs != null)
							{
								UnsubscribeFromCatalogNotifications(catalogs);
								catalogs.ForEach(catalog => catalog.Dispose());
							}
						}
					}
				}
			}
		}

		private void RaiseChangedEvent(
				Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions,
				Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions)
		{
			if (_onChanged == null || Changed == null)
			{
				return;
			}

			var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
			var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);

			_onChanged.Invoke(new ComposablePartCatalogChangeEventArgs(added, removed, null));
		}

		public void OnChanged(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			var changedEvent = Changed;
			if (changedEvent != null)
			{
				changedEvent(sender, e);
			}
		}

		private void RaiseChangingEvent(
			 Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions,
			 Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions,
			 AtomicComposition atomicComposition)
		{
			if (_onChanging == null || Changing == null)
			{
				return;
			}
			var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
			var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);

			_onChanging.Invoke(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
		}

		public void OnChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			var changingEvent = Changing;
			if (changingEvent != null)
			{
				changingEvent(sender, e);
			}
		}

		private void OnContainedCatalogChanged(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			if (_onChanged == null || Changed == null)
			{
				return;
			}

			_onChanged.Invoke(e);
		}

		private void OnContainedCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			if (_onChanging == null || Changing == null)
			{
				return;
			}

			_onChanging.Invoke(e);
		}

		private void SubscribeToCatalogNotifications(ComposablePartCatalog catalog)
		{
			var notifyCatalog = catalog as INotifyComposablePartCatalogChanged;
			if (notifyCatalog != null)
			{
				notifyCatalog.Changed += OnContainedCatalogChanged;
				notifyCatalog.Changing += OnContainedCatalogChanging;
			}
		}

		private void SubscribeToCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
		{
			foreach (var catalog in catalogs)
			{
				SubscribeToCatalogNotifications(catalog);
			}
		}

		private void UnsubscribeFromCatalogNotifications(ComposablePartCatalog catalog)
		{
			var notifyCatalog = catalog as INotifyComposablePartCatalogChanged;
			if (notifyCatalog != null)
			{
				notifyCatalog.Changed -= OnContainedCatalogChanged;
				notifyCatalog.Changing -= OnContainedCatalogChanging;
			}
		}

		private void UnsubscribeFromCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
		{
			foreach (var catalog in catalogs)
			{
				UnsubscribeFromCatalogNotifications(catalog);
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