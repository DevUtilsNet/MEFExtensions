using System.ComponentModel.Composition.Hosting;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.DataAnnotations
{
	[TestClass]
	public class DataAnnotationsComposablePartCatalogFactoryTests
	{
		[TestMethod]
		public void GetComposablePartCatalogTest01()
		{
			var factory = new DataAnnotationsComposablePartCatalogFactory(new AggregateCatalog());

			var catalogs = new []
			{
				factory.GetComposablePartCatalog("1"),
				factory.GetComposablePartCatalog("2")
			};

			Assert.IsTrue(catalogs.SequenceEqual(new[]
			{
				factory.GetComposablePartCatalog("1"),
				factory.GetComposablePartCatalog("2")
			}));
		}
	}
}
