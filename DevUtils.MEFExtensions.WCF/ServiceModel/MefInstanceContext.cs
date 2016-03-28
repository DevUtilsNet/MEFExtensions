using System;
using System.ServiceModel;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;

namespace DevUtils.MEFExtensions.WCF.ServiceModel
{
	sealed class MefInstanceContext
				: IDisposable
				, IExtension<InstanceContext>
	{
		private readonly ICompositionScopeManager _manager;

		public MefInstanceContext(ICompositionScopeManager manager)
		{
			_manager = manager;
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			_manager.Dispose();
		}

		#endregion

		#region Implementation of IExtension<InstanceContext>

		public void Attach(InstanceContext owner)
		{
		}

		public void Detach(InstanceContext owner)
		{
		}

		#endregion
	}
}