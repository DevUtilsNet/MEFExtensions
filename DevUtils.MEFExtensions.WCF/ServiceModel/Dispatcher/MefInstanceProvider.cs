using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.WCF.ServiceModel.Activation;

namespace DevUtils.MEFExtensions.WCF.ServiceModel.Dispatcher
{
	sealed class MefInstanceProvider<T>
				: IInstanceProvider
	{
		private readonly ICompositionScopeManager _scopeManager;

		public MefInstanceProvider(ICompositionScopeManager scopeManager)
		{
			_scopeManager = scopeManager;
		}

		#region Implementation of IInstanceProvider

		public object GetInstance(InstanceContext instanceContext)
		{
			var ret = GetInstance(instanceContext, null);
			return ret;
		}

		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			var manager = _scopeManager.CreateCompositionScopeManager(MefServiceHostFactory.InstanceScopeName);

			var ret = manager.Container.GetExportedValue<T>();

			instanceContext.Extensions.Add(new MefInstanceContext(manager));

			return ret;
		}

		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
			var ctx = instanceContext.Extensions.Find<MefInstanceContext>();
			ctx?.Dispose();
		}

		#endregion
	}
}