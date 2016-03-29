using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.WCF.ServiceModel.Description;

namespace DevUtils.MEFExtensions.WCF.ServiceModel.Activation
{
	/// <summary> A MEF service host factory. This class cannot be inherited. </summary>
	public sealed class MefServiceHostFactory
				 : ServiceHostFactory
	{
		/// <summary> Name of the instance scope. </summary>
		public static string InstanceScopeName { get; set; } = InstanceExportAttribute.ScopeName;

		/// <summary> Gets or sets the name of the service host scope. </summary>
		///
		/// <value> The name of the service host scope. </value>
		public static string ServiceHostScopeName { get; set; } = ServiceHostExportAttribute.ScopeName;

		/// <summary> Gets or sets the manager for application scope. </summary>
		///
		/// <value> The application scope manager. </value>
		public static ICompositionScopeManager ApplicationScopeManager { get; set; }

		private static void AddBehavior(Type serviceType, ServiceHost serviceHost, ICompositionScopeManager scopeManager)
		{
			serviceHost.Description.Behaviors.Add(new MefDependencyInjectionServiceBehavior(serviceType, scopeManager));
		}

		private static void OnOpening(Type serviceType, object sender)
		{
			var scopeManager = ApplicationScopeManager.CreateCompositionScopeManager(ServiceHostScopeName);

			AddBehavior(serviceType, (ServiceHost)sender, scopeManager);
		}

		private static void OnClosing(object sender, EventArgs eventArgs)
		{
			var behavior = ((ServiceHost)sender).Description.Behaviors.Find<MefDependencyInjectionServiceBehavior>();
			behavior?.ScopeManager.Dispose();
		}

		#region Overrides of ServiceHostFactory

		/// <summary>
		/// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address. 
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.
		/// </returns>
		/// <param name="serviceType">Specifies the type of service to host. </param><param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param>
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			if (ApplicationScopeManager == null)
			{
				throw new InvalidOperationException("The MefServiceHostFactory.RootScopeManager static property must be set before services can be instantiated.");
			}

			var ret = base.CreateServiceHost(serviceType, baseAddresses);

			if (serviceType.GetCustomAttributes(typeof(InstanceExportAttribute)).Any())
			{
				ret.Opening += (s, a) => OnOpening(serviceType, s);
				ret.Closing += OnClosing;
			}

			return ret;
		}

		#endregion

		internal static ServiceHost CreateServiceHost2(Type serviceType, Uri[] baseAddresses)
		{
			var ret = new MefServiceHostFactory().CreateServiceHost(serviceType, baseAddresses);
			return ret;
		}
	}
}