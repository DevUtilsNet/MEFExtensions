using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	[Export]
	class CompositionScopeManagerDisposeGuard
		: IDisposable
	{
		private readonly List<CompositionScopeManager> _managers = new List<CompositionScopeManager>();

		public CompositionScopeManagerDisposeGuard()
		{
		}

		public void Add(CompositionScopeManager manager)
		{
			lock (_managers)
			{
				_managers.Add(manager);
			}
		}

		public void Remove(CompositionScopeManager manager)
		{
			lock (_managers)
			{
				_managers.Remove(manager);
			}
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			lock (_managers)
			{
				foreach (var item in _managers.ToArray())
				{
					item.Dispose();
				}

				_managers.Clear();
			}
		}

		#endregion
	}
}