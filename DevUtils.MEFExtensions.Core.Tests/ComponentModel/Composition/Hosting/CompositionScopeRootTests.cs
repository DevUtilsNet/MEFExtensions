using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.Hosting
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class TestExportAttribute
		: ApplicationExportAttribute
	{
		public const string TestScopeName = "Test";

		public TestExportAttribute()
			: base(TestScopeName, (string)null)
		{
		}

		public TestExportAttribute(Type contractType)
			: base(TestScopeName, null, contractType)
		{
		}
	}

	interface ICompositionScopeRootTestsExport { bool DisposeWasCalling { get; } }
	interface ICompositionScopeRootTestsExport0 : ICompositionScopeRootTestsExport { }
	interface ICompositionScopeRootTestsExport1 : ICompositionScopeRootTestsExport { }
	interface ICompositionScopeRootTestsExport2 : ICompositionScopeRootTestsExport { }

	[TestExport(typeof(ICompositionScopeRootTestsExport2))]
	[ScopedExport(typeof(ICompositionScopeRootTestsExport0))]
	[ApplicationExport(typeof(ICompositionScopeRootTestsExport1))]
	class CompositionScopeRootTestsExportAll
		: IDisposable
		, ICompositionScopeRootTestsExport0
		, ICompositionScopeRootTestsExport1
		, ICompositionScopeRootTestsExport2
	{
		public bool DisposeWasCalling { get; set; }

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion
	}

	[ScopedExport]
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

	[ApplicationExport]
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

	[TestExport]
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

	[Export]
	[ApplicationExport(typeof(IApplicationModule))]
	class CompositionScopeRootTestsExportApplication
		: IDisposable
		, IApplicationModule
	{
		public bool DisposeWasCalling { get; set; }
		public bool InitializeWasCalling { get; set; }

		#region Implementation of IDisposable

		public void Dispose()
		{
			DisposeWasCalling = true;
		}

		#endregion

		#region Implementation of IScopeModule

		public void Initialize()
		{
			InitializeWasCalling = true;
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
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName))
				{
					Assert.AreEqual(ApplicationExportAttribute.ApplicationScopeName, manager1.ScopeName);
					Assert.AreEqual(new ApplicationExportAttribute().ScopeFullName, manager1.ScopeFullName);
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.TestScopeName))
					{
						Assert.AreEqual(TestExportAttribute.TestScopeName, manager2.ScopeName);
						Assert.AreEqual(new TestExportAttribute().ScopeFullName, manager2.ScopeFullName);

						Assert.IsTrue(manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeName).SequenceEqual(
							new[]
							{
								TestExportAttribute.TestScopeName,
								ApplicationExportAttribute.ApplicationScopeName,
								null
							}));
						Assert.IsTrue(manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeFullName).SequenceEqual(
							new[]
							{
								new TestExportAttribute().ScopeFullName,
								new ApplicationExportAttribute().ScopeFullName,
								null
							}));

						Assert.AreEqual(manager1, manager2.ParentManager);
						Assert.AreEqual(manager0, manager1.ParentManager);
						Assert.IsNull(manager0.ParentManager);
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
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName))
				{
					value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
					CompositionScopeRootTestsExport2 value2;
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.TestScopeName))
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
				var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName);

				value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
				var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.TestScopeName);
				value2 = manager2.Container.GetExportedValue<CompositionScopeRootTestsExport2>();
			}

			Assert.IsTrue(value2.DisposeWasCalling);
			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);
		}

		[TestMethod]
		public void CreateRootScopeManagerTest05()
		{
			ICompositionScopeRootTestsExport0 value0;
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				value0 = manager0.Container.GetExportedValue<ICompositionScopeRootTestsExport0>();
				ICompositionScopeRootTestsExport1 value1;
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName))
				{
					value1 = manager1.Container.GetExportedValue<ICompositionScopeRootTestsExport1>();
					ICompositionScopeRootTestsExport2 value2;
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.TestScopeName))
					{
						value2 = manager2.Container.GetExportedValue<ICompositionScopeRootTestsExport2>();
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
		public void CreateRootScopeManagerTest06()
		{
			CompositionScopeRootTestsExport0 value0;
			CompositionScopeRootTestsExport1 value1;
			CompositionScopeRootTestsExport2 value2;

			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly()))))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName);

				value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
				var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.TestScopeName, false);
				value2 = manager2.Container.GetExportedValue<CompositionScopeRootTestsExport2>();
			}

			Assert.IsFalse(value2.DisposeWasCalling);
			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);
		}

		[TestMethod]
		public void CreateRootScopeManagerTest07()
		{
			var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly())));
			var value0 = manager0.Container.GetExportedValue<ICompositionScopeRootTestsExport0>();

			var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ApplicationScopeName);

			var value1 = manager1.Container.GetExportedValue<ICompositionScopeRootTestsExport1>();

			manager0.Dispose();

			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);

			manager1.Dispose();
		}

		[TestMethod]
		public void CreateApplicationScopeManagerTest01()
		{
			var manager0 = CompositionScopeRoot.CreateApplicationScopeManager(new DataAnnotationsComposablePartCatalogFactory(new AssemblyCatalog(Assembly.GetExecutingAssembly())));
			var value = manager0.Container.GetExportedValue<CompositionScopeRootTestsExportApplication>();

			Assert.IsFalse(value.DisposeWasCalling);
			Assert.IsTrue(value.InitializeWasCalling);

			manager0.Dispose();

			Assert.IsTrue(value.DisposeWasCalling);
		}
	}
}