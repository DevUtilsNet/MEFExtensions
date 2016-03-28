using System;
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
		/// <summary> Gets or sets the composition scope manager action. </summary>
		///
		/// <value> The composition scope manager action. </value>
		public static Func<ServiceHost, ICompositionScopeManager> CompositionScopeManagerAction { get; set; }

		private static void AddBehavior(Type serviceType, ServiceHost serviceHost)
		{
			var scopeManager = CompositionScopeManagerAction(serviceHost);
			serviceHost.Description.Behaviors.Add(new MefDependencyInjectionServiceBehavior(serviceType, scopeManager));
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
			if (CompositionScopeManagerAction == null)
			{
				throw new InvalidOperationException("The MefServiceHostFactory.CompositionScopeManagerAction static property must be set before services can be instantiated.");
			}

			var ret = base.CreateServiceHost(serviceType, baseAddresses);

			if (serviceType.GetCustomAttribute(typeof(InstanceExportAttribute)) != null)
			{
				ret.Opening += (s, a) => AddBehavior(serviceType, (ServiceHost)s);
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