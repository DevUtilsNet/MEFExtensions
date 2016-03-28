using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.WCF.ServiceModel.Dispatcher;

namespace DevUtils.MEFExtensions.WCF.ServiceModel.Description
{
	sealed class MefDependencyInjectionServiceBehavior
				: IServiceBehavior
	{
		private readonly Type _serviceType;
		private readonly ICompositionScopeManager _scopeManager;

		public MefDependencyInjectionServiceBehavior(Type serviceType, ICompositionScopeManager scopeManager)
		{
			_serviceType = serviceType;
			_scopeManager = scopeManager;
		}

		#region Implementation of IServiceBehavior

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
																		 BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			var contracts = serviceDescription.Endpoints.Where(w => w.Contract.ContractType.IsAssignableFrom(_serviceType)).Select(s => s.Contract.Name).ToArray();

			var type = typeof(MefInstanceProvider<>);
			type = type.MakeGenericType(_serviceType);
			var constructor = type.GetConstructor(new[] { typeof(ICompositionScopeManager) });

			var instanceProvider = (IInstanceProvider)constructor.Invoke(new object[] { _scopeManager });

			foreach (var item in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany(w => w.Endpoints))
			{
				if (!contracts.Contains(item.ContractName))
				{
					continue;
				}
				item.DispatchRuntime.InstanceProvider = instanceProvider;
			}
		}

		#endregion
	}
}