using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.Hosting
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	class TestExportAttribute
		: ApplicationExportAttribute
	{
		public new static readonly string ScopeName = "Test";

		public TestExportAttribute()
			: base(new ScopeName(ScopeName))
		{
		}

		public TestExportAttribute(Type contractType)
			: base(new ScopeName(ScopeName), contractType)
		{
		}
	}

	class TestModuleExportAttribute
		: ApplicationModuleExportAttribute
	{
		public TestModuleExportAttribute()
			: base(new ScopeName(TestExportAttribute.ScopeName))
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

	[ApplicationModuleExport]
	class CompositionScopeRootTestsExportApplicationModule
		: IDisposable
		, IApplicationModule
	{
		public static int CtorCount { get; set; }
		public static int DisposeCount { get; set; }
		public static int InitializeCount { get; set; }

		public CompositionScopeRootTestsExportApplicationModule()
		{
			++CtorCount;
		}

		public static void Reset()
		{
			CtorCount = 0;
			DisposeCount = 0;
			InitializeCount = 0;
		}

		#region Implementation of IScopeModule

		public void Initialize()
		{
			++InitializeCount;
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{
			++DisposeCount;
		}

		#endregion
	}

	[TestModuleExport]
	class CompositionScopeRootTestsExportTestModule
		: IDisposable
		, IApplicationModule
	{
		public static int CtorCount { get; set; }
		public static int DisposeCount { get; set; }
		public static int InitializeCount { get; set; }

		public CompositionScopeRootTestsExportTestModule()
		{
			++CtorCount;
		}

		public static void Reset()
		{
			CtorCount = 0;
			DisposeCount = 0;
			InitializeCount = 0;
		}

		#region Implementation of IScopeModule

		public void Initialize()
		{
			++InitializeCount;
		}

		#endregion

		#region Implementation of IDisposable

		public void Dispose()
		{
			++DisposeCount;
		}

		#endregion
	}

	[TestClass]
	public class CompositionScopeRootTests
	{
		private readonly ComposablePartCatalog _catalog = new TypeCatalog(
			typeof(CompositionScopeRootTestsExport0),
			typeof(CompositionScopeRootTestsExport1),
			typeof(CompositionScopeRootTestsExport2),
			typeof(CompositionScopeRootTestsExportAll),
			typeof(CompositionScopeRootTestsExportTestModule),
			typeof(CompositionScopeRootTestsExportApplicationModule));

		[TestMethod]
		public void CreateRootScopeManagerTest01()
		{
			using (var manager = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				Assert.IsNotNull(manager);
				Assert.IsNull(manager.ScopeName);
				Assert.IsNull(manager.ScopeFullName.FullName);

				Assert.IsNotNull(manager.Catalog);
				Assert.IsNotNull(manager.Container);

				Assert.IsFalse(manager.Container.GetExportedValues<CompositionScopeRootTestsExport1>().Any());
				Assert.IsInstanceOfType(manager.Container.GetExportedValue<CompositionScopeRootTestsExport0>(), typeof(CompositionScopeRootTestsExport0));
			}
		}

		[TestMethod]
		public void CreateRootScopeManagerTest02()
		{
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName))
				{
					Assert.AreEqual(ApplicationExportAttribute.ScopeName, manager1.ScopeName);
					Assert.AreEqual(new ApplicationExportAttribute().ScopeFullName, manager1.ScopeFullName.FullName);
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.ScopeName))
					{
						Assert.AreEqual(TestExportAttribute.ScopeName, manager2.ScopeName);
						Assert.AreEqual(new TestExportAttribute().ScopeFullName, manager2.ScopeFullName.FullName);

						var s1 = manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeName);
						Assert.IsTrue(s1.SequenceEqual(
							new[]
							{
								TestExportAttribute.ScopeName,
								ApplicationExportAttribute.ScopeName,
								null
							}));
						Assert.IsTrue(manager2.Container.GetExportedValues<ICompositionScopeManager>().Select(s => s.ScopeFullName).SequenceEqual(
							new[]
							{
								new ScopeName(new TestExportAttribute().ScopeFullName),
								new ScopeName(new ApplicationExportAttribute().ScopeFullName),
								default(ScopeName)
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
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				CompositionScopeRootTestsExport1 value1;
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName))
				{
					value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
					CompositionScopeRootTestsExport2 value2;
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.ScopeName))
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

			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName);

				value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
				var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.ScopeName);
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
			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				value0 = manager0.Container.GetExportedValue<ICompositionScopeRootTestsExport0>();
				ICompositionScopeRootTestsExport1 value1;
				using (var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName))
				{
					value1 = manager1.Container.GetExportedValue<ICompositionScopeRootTestsExport1>();
					ICompositionScopeRootTestsExport2 value2;
					using (var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.ScopeName))
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

			using (var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog)))
			{
				value0 = manager0.Container.GetExportedValue<CompositionScopeRootTestsExport0>();
				var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName);

				value1 = manager1.Container.GetExportedValue<CompositionScopeRootTestsExport1>();
				var manager2 = manager1.CreateCompositionScopeManager(TestExportAttribute.ScopeName, null, false);
				value2 = manager2.Container.GetExportedValue<CompositionScopeRootTestsExport2>();
			}

			Assert.IsFalse(value2.DisposeWasCalling);
			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);
		}

		[TestMethod]
		public void CreateRootScopeManagerTest07()
		{
			var manager0 = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog));
			var value0 = manager0.Container.GetExportedValue<ICompositionScopeRootTestsExport0>();

			var manager1 = manager0.CreateCompositionScopeManager(ApplicationExportAttribute.ScopeName);

			var value1 = manager1.Container.GetExportedValue<ICompositionScopeRootTestsExport1>();

			manager0.Dispose();

			Assert.IsTrue(value1.DisposeWasCalling);
			Assert.IsTrue(value0.DisposeWasCalling);

			manager1.Dispose();
		}

		[TestMethod]
		public void CreateApplicationScopeManagerTest01()
		{
			CompositionScopeRootTestsExportTestModule.Reset();
			CompositionScopeRootTestsExportApplicationModule.Reset();

			var manager0 = CompositionScopeRoot.CreateApplicationScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog));
			Assert.AreEqual(0, CompositionScopeRootTestsExportTestModule.CtorCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportTestModule.InitializeCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportTestModule.DisposeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.InitializeCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportApplicationModule.DisposeCount);

			var manager1 = manager0.CreateCompositionScopeManager(TestExportAttribute.ScopeName);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.InitializeCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportTestModule.DisposeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.InitializeCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportApplicationModule.DisposeCount);

			manager1.Dispose();
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.InitializeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.DisposeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.InitializeCount);
			Assert.AreEqual(0, CompositionScopeRootTestsExportApplicationModule.DisposeCount);

			manager0.Dispose();
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.InitializeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportTestModule.DisposeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.CtorCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.InitializeCount);
			Assert.AreEqual(1, CompositionScopeRootTestsExportApplicationModule.DisposeCount);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateCreateCompositionScopeManagerTest01()
		{
			var manager = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog));
			manager.CreateCompositionScopeManager("*");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateCreateCompositionScopeManagerTest02()
		{
			var manager = CompositionScopeRoot.CreateRootScopeManager(new DataAnnotationsComposablePartCatalogFactory(_catalog));
			manager.CreateCompositionScopeManager("**");
		}
	}
}