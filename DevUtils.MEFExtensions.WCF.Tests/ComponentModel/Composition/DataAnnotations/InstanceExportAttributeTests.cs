using DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.WCF.Tests.ComponentModel.Composition.DataAnnotations
{
	[TestClass]
	public class InstanceExportAttributeTests
	{
		[TestMethod]
		public void InstanceExportAttributeTest01()
		{
			var att = new InstanceExportAttribute();

			Assert.AreEqual("Application/ServiceHost/Instance", att.ScopeFullName);
		}
	}
}
