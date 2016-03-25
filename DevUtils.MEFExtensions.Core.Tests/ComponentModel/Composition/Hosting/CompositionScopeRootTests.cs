using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.Hosting
{
	interface ICompositionScopeRootTestsExport { }
	interface ICompositionScopeRootTestsExport0 : ICompositionScopeRootTestsExport { }
	interface ICompositionScopeRootTestsExport1 : ICompositionScopeRootTestsExport { }
	interface ICompositionScopeRootTestsExport2 : ICompositionScopeRootTestsExport { }

	[ScopedExport(null, typeof(ICompositionScopeRootTestsExport0))]
	[ScopedExport("1", typeof(ICompositionScopeRootTestsExport1))]
	[ScopedExport("1/2", typeof(ICompositionScopeRootTestsExport2))]
	class CompositionScopeRootTestsExportAll
		: IDisposable
		, ICompositionScopeRootTestsExport0
		, ICompositionScopeRootTestsExport1
		, ICompositionScopeRootTestsExport2
	{
		public bool DisposeWasCalling { get; set; }

		public CompositionScopeRootTestsExportAll()
		{
			
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	[ScopedExport(null)]
	class CompositionScopeRootTestsExport0
		: IDisposable
	{
		public bool DisposeWasCalling { get; set; }

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	[ScopedExport("1")]
	class CompositionScopeRootTestsExport1
		: IDisposable
	{
		public bool DisposeWasCalling { get; set; }

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	[ScopedExport("1/2")]
	class CompositionScopeRootTestsExport2
		: IDisposable
	{
		public bool DisposeWasCalling { get; set; }

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	

	[TestClass]
	public class CompositionScopeRootTests
	{
		[TestMethod]
		public void CreateRootScopeManagerTest01()
		{
			using (var manager = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				Assert.IsNotNull(manager);
				Assert.IsNull(manager.ScopeName);
				Assert.IsNull(manager.ScopeFullName);

				Assert.IsNotNull(manager.Catalog);
				Assert.IsNotNull(manager.Container);

				Assert.IsFalse(manager.Container.GetExportedValues<CompositionScopeRootTestsExport1>().Any());
				Assert.IsInstanceOfType(manager.Container.GetExportedValue<CompositionScopeRootTestsExport0>(), typeof(CompositionScopeRootTestsExport0));
			}
		}

		[TestMethod]
		public void CreateRootScopeManagerTest02()
		{
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				using (var manager1 = manager0.CreateCompositionScopeManager("1"))
				{
					Assert.AreEqual("1", manager1.ScopeName);
					Assert.AreEqual("1", manager1.ScopeFullName);
					using (var manager2 = manager1.CreateCompositionScopeManager("2"))
					{
						Assert.AreEqual("2", manager2.ScopeName);
						Assert.AreEqual("1/2", manager2.ScopeFullName);

						Assert.IsTrue(manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeName).SequenceEqual(new [] {"2", "1", null}));
						Assert.IsTrue(manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeFullName).SequenceEqual(new[] { "1/2", "1", null }));
					}
				}
			}
		}

		[TestMethod]
		public void CreateRootScopeManagerTest03()
		{
			CompositionScopeRootTestsExport0 value0;
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				CompositionScopeRootTestsExport1 value1;
				using (var manager1 = manager0.CreateCompositionScopeManager("1"))
				{
					value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
					CompositionScopeRootTestsExport2 value2;
					using (var manager2 = manager1.CreateCompositionScopeManager("2"))
					{
						value2 = manager2.Container.GetExportedValue<CompositionScopeRootTestsExport2>();
					}
					Assert.IsTrue(value2.DisposeWasCalling);
					Assert.IsFalse(value1.DisposeWasCalling);
					Assert.IsFalse(value0.DisposeWasCalling);
				}
				Assert.IsTrue(value1.DisposeWasCalling);
				Assert.IsFalse(value0.DisposeWasCalling);
			}
			Assert.IsTrue(value0.DisposeWasCalling);
		}

		[TestMethod]
		public void CreateRootScopeManagerTest04()
		{
			CompositionScopeRootTestsExport0 value0;
			CompositionScopeRootTestsExport1 value1;
			CompositionScopeRootTestsExport2 value2;

			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				var manager1 = manager0.CreateCompositionScopeManager("1");

				value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
				var manager2 = manager1.CreateCompositionScopeManager("2");
				value2 = manager2.Container.GetExportedValue<CompositionScopeRootTestsExport2>();
			}

			Assert.IsTrue(value2.DisposeWasCalling);
			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);
		}

		[TestMethod]
		public void CreateRootScopeManagerTest05()
		{
			ICompositionScopeRootTestsExport value0;
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				value0 = manager0.Container.GetExportedValue<ICompositionScopeRootTestsExport0>();
				using (var manager1 = manager0.CreateCompositionScopeManager("1"))
				{
					value0 = manager1.Container.GetExportedValue<ICompositionScopeRootTestsExport1>();
					using (var manager2 = manager1.CreateCompositionScopeManager("2"))
					{
						value0 = manager2.Container.GetExportedValue<ICompositionScopeRootTestsExport2>();
					}
				}
			}
		}
	}
}