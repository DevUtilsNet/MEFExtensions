using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.WCF.ComponentModel.Composition.Primitives;
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
		public static MefServiceHostFactoryTestsService Instance { get; set; }

		public bool DisposeWasCalling { get; set; }
		public ICompositionScopeManager Manager { get; set; }
		public MefServiceHostFactoryTestsInstanceModule Module { get; }

		[ImportingConstructor]
		public MefServiceHostFactoryTestsService(
			[Import] ICompositionScopeManager manager,
			[Import] MefServiceHostFactoryTestsInstanceModule module)
		{
			Manager = manager;
			Module = module;
			Instance = this;
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

	[ServiceHostExport(typeof(IServiceHostModule))]
	public class MefServiceHostFactoryTestsServiceHostModule 
		: IServiceHostModule
		, IDisposable
	{
		public bool DisposeWasCalling { get; set; }
		public bool InitializeWasCalling { get; set; }

		public static MefServiceHostFactoryTestsServiceHostModule Instance { get; set; }

		public MefServiceHostFactoryTestsServiceHostModule()
		{
			Instance = this;
		}

		#region Implementation of IScopeModule

		public void Initialize()
		{
			InitializeWasCalling = true;
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	[Export]
	[InstanceExport(typeof(IInstanceModule))]
	public class MefServiceHostFactoryTestsInstanceModule
		: IInstanceModule
		, IDisposable
	{
		public bool DisposeWasCalling { get; set; }
		public bool InitializeWasCalling { get; set; }
		public ICompositionScopeManager Manager { get; }

		public static MefServiceHostFactoryTestsInstanceModule Instance { get; set; }

		[ImportingConstructor]
		public MefServiceHostFactoryTestsInstanceModule(
			[Import] ICompositionScopeManager manager)
		{
			Manager = manager;
			Instance = this;
		}

		#region Implementation of IScopeModule

		public void Initialize()
		{
			InitializeWasCalling = true;
		}

		#endregion

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

		public MefServiceHostFactoryTests()
		{
			MefServiceHostFactory.ApplicationScopeManager = _applicationManager;
		}

		[TestMethod]
		public void CreateServiceHostTest01()
		{
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
					Assert.IsFalse(MefServiceHostFactoryTestsService.Instance.DisposeWasCalling);
				}
				Thread.Sleep(100);
				Assert.IsTrue(MefServiceHostFactoryTestsService.Instance.DisposeWasCalling);
			}
		}

		[TestMethod]
		public void CreateServiceHostTest02()
		{
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

		[TestMethod]
		public void CreateServiceHostTest03()
		{
			MefServiceHostFactoryTestsServiceHostModule.Instance = null;

			using (var host = MefServiceHostFactory.CreateServiceHost2(typeof(MefServiceHostFactoryTestsService), new[] { new Uri("net.pipe://localhost/") }))
			{
				var endPoint = new ServiceEndpoint(
					ContractDescription.GetContract(typeof(IMefServiceHostFactoryTestsContract)),
					new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MefServiceHostFactoryTests"));

				host.AddServiceEndpoint(endPoint);

				host.Open();
				Assert.IsTrue(MefServiceHostFactoryTestsServiceHostModule.Instance.InitializeWasCalling);

				using (var client = new MefServiceHostFactoryTestsClient(endPoint))
				{
					client.Test();
					Assert.IsTrue(MefServiceHostFactoryTestsInstanceModule.Instance.InitializeWasCalling);

					Assert.AreEqual(MefServiceHostFactoryTestsInstanceModule.Instance, MefServiceHostFactoryTestsService.Instance.Module);
					Assert.AreEqual(MefServiceHostFactoryTestsInstanceModule.Instance.Manager, MefServiceHostFactoryTestsService.Instance.Manager);
				}
				Thread.Sleep(100);
				Assert.IsTrue(MefServiceHostFactoryTestsInstanceModule.Instance.DisposeWasCalling);
			}
			Assert.IsTrue(MefServiceHostFactoryTestsServiceHostModule.Instance.DisposeWasCalling);
		}
	}
}