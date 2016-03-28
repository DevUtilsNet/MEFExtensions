using System.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Core.Collections.Generic.Extensions;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions
{
	static class CompositionContainerExtensions
	{
		public static void InitializeModules<T>(this CompositionContainer container) where T : IScopeModule
		{
			container.GetExportedValues<T>().ForEach(f => f.Initialize());
		}
	}
}