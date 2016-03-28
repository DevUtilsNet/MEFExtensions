using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.WCF.ServiceModel.Activation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.WCF.Tests.ServiceModel.Activation
{
	[ServiceContract]
	public interface IMefServiceHostFactoryTestsContract
	{
		[OperationContract]
		void Test();
	}

	[InstanceExport]
	public class MefServiceHostFactoryTestsService 
		: IMefServiceHostFactoryTestsContract
		, IDisposable
	{
		public static bool DisposeWasCalling { get; set; }

		public MefServiceHostFactoryTestsService()
		{
			DisposeWasCalling = false;
		}

		public void Test()
		{
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	public class MefServiceHostFactoryTestsRawService
		: IMefServiceHostFactoryTestsContract
	{
		public static bool DisposeWasCalling { get; set; }

		public MefServiceHostFactoryTestsRawService()
		{
			DisposeWasCalling = false;
		}

		public void Test()
		{
		}
	}

	public class MefServiceHostFactoryTestsClient : ClientBase<IMefServiceHostFactoryTestsContract>
	{
		public MefServiceHostFactoryTestsClient(ServiceEndpoint endPoint)
				: base(endPoint)
		{
		}

		public void Test()
		{
			Channel.Test();
		}
	}

	[TestClass]
	public class MefServiceHostFactoryTests
	{
		private readonly ICompositionScopeManager _applicationManager = CompositionScopeRoot.CreateApplicationScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly())));

		private ICompositionScopeManager CompositionScopeManagerAction(ServiceHost serviceHost)
		{
			return _applicationManager;
		}

		[TestMethod]
		public void CreateServiceHostTest01()
		{
			MefServiceHostFactory.CompositionScopeManagerAction = CompositionScopeManagerAction;

			using (var host = MefServiceHostFactory.CreateServiceHost2(typeof(MefServiceHostFactoryTestsService), new[] { new Uri("net.pipe://localhost/") }))
			{
				var endPoint = new ServiceEndpoint(
					ContractDescription.GetContract(typeof (IMefServiceHostFactoryTestsContract)),
					new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MefServiceHostFactoryTests"));

				host.AddServiceEndpoint(endPoint);

				host.Open();

				using (var client = new MefServiceHostFactoryTestsClient(endPoint))
				{
					client.Test();
					Assert.IsFalse(MefServiceHostFactoryTestsService.DisposeWasCalling);
				}
				Thread.Sleep(100);
				Assert.IsTrue(MefServiceHostFactoryTestsService.DisposeWasCalling);
			}
		}

		[TestMethod]
		public void CreateServiceHostTest02()
		{
			MefServiceHostFactory.CompositionScopeManagerAction = CompositionScopeManagerAction;

			using (var host = MefServiceHostFactory.CreateServiceHost2(typeof(MefServiceHostFactoryTestsRawService), new[] { new Uri("net.pipe://localhost/") }))
			{
				var endPoint = new ServiceEndpoint(
					ContractDescription.GetContract(typeof(IMefServiceHostFactoryTestsContract)),
					new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MefServiceHostFactoryTests"));

				host.AddServiceEndpoint(endPoint);

				host.Open();

				using (var client = new MefServiceHostFactoryTestsClient(endPoint))
				{
					client.Test();
				}
			}
		}
	}
}