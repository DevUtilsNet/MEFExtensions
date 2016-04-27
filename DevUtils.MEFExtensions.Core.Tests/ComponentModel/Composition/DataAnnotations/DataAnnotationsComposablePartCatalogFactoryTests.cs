using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.DataAnnotations
{
	class AnyAllExportAttributeTest
		: AnyAllExportAttribute
	{
		public new static readonly string ScopeName = "AnyAllExportAttributeTest";

		public AnyAllExportAttributeTest() 
			: base(new ScopeName(ScopeName))
		{
		}
	}

	class AnyOneExportAttributeTest
		: AnyOneExportAttribute
	{
		public new static readonly string ScopeName = "AnyOneExportAttributeTest";

		public AnyOneExportAttributeTest()
			: base(new ScopeName(ScopeName))
		{
		}
	}

	[AnyAllExportAttributeTest]
	class AnyAllExportTest
	{
		 
	}

	[AnyOneExportAttributeTest]
	class AnyOneExportTest
	{

	}

	[TestClass]
	public class DataAnnotationsComposablePartCatalogFactoryTests
	{
		private readonly ComposablePartCatalog _catalog = new TypeCatalog(typeof(AnyAllExportTest), typeof(AnyOneExportTest)); 

		[TestMethod]
		public void GetComposablePartCatalogTest01()
		{
			var factory = new DataAnnotationsComposablePartCatalogFactory(new AggregateCatalog());

			var catalogs = new []
			{
				factory.GetComposablePartCatalog(new ScopeName("1")),
				factory.GetComposablePartCatalog(new ScopeName("2"))
			};

			Assert.IsTrue(catalogs.SequenceEqual(new[]
			{
				factory.GetComposablePartCatalog(new ScopeName("1")),
				factory.GetComposablePartCatalog(new ScopeName("2"))
			}));
		}

		[TestMethod]
		public void GetComposablePartCatalogTest02()
		{
			var factory = new DataAnnotationsComposablePartCatalogFactory(_catalog);

			var catalog = factory.GetComposablePartCatalog(new ScopeName(AnyAllExportAttributeTest.ScopeName).CombainBefore("1", "2", "3", "4"));

			Assert.AreEqual(typeof(AnyAllExportTest).FullName, catalog.Select(s => s.ExportDefinitions.Select(s2 => s2.Metadata[CompositionConstants.ExportTypeIdentityMetadataName]).Single()).Single());

			catalog = factory.GetComposablePartCatalog(new ScopeName(AnyAllExportAttributeTest.ScopeName));

			Assert.IsFalse(catalog.Any());
		}

		[TestMethod]
		public void GetComposablePartCatalogTest03()
		{
			var factory = new DataAnnotationsComposablePartCatalogFactory(_catalog);

			var catalog = factory.GetComposablePartCatalog(new ScopeName(AnyOneExportAttributeTest.ScopeName).CombainBefore("1"));

			Assert.AreEqual(typeof(AnyOneExportTest).FullName, catalog.Select(s => s.ExportDefinitions.Select(s2 => s2.Metadata[CompositionConstants.ExportTypeIdentityMetadataName]).Single()).Single());

			catalog = factory.GetComposablePartCatalog(new ScopeName(AnyAllExportAttributeTest.ScopeName));

			Assert.IsFalse(catalog.Any());
		}
	}
}