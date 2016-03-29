using System.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Core.Collections.Generic.Extensions;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions
{
	static class CompositionContainerExtensions
	{
		public static void InitializeModules(this CompositionContainer container, string fullNameScope)
		{
			var modules = container.GetExportedValues<IScopeModule>(fullNameScope);
			modules.ForEach(f => f.Initialize());
		}
	}
}