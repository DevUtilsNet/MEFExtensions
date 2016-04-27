using System.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Core.Collections.Generic.Extensions;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions
{
	static class CompositionContainerExtensions
	{
		public static void InitializeModules(this CompositionContainer container, ScopeName nameScope)
		{
			var modules = container.GetExportedValues<IScopeModule>(nameScope.FullName);
			modules.ForEach(f => f.Initialize());
		}
	}
}