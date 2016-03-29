using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	/// <summary> A composition scope root. </summary>
	public static class CompositionScopeRoot
	{
		/// <summary> Creates root scope. </summary>
		///
		/// <returns> The new root scope. </returns>
		public static ICompositionScopeManager CreateRootScopeManager(IComposablePartCatalogFactory catalogFactory)
		{
			var ret = new CompositionScopeManager(catalogFactory);
			return ret;
		}

		/// <summary> Creates application scope manager. </summary>
		///
		/// <param name="catalogFactory"> The catalog factory. </param>
		///
		/// <returns> The new application scope manager. </returns>
		public static ICompositionScopeManager CreateApplicationScopeManager(IComposablePartCatalogFactory catalogFactory)
		{
			var ret = new CompositionScopeManager(ApplicationExportAttribute.ScopeName, catalogFactory);
			ret.InitializeModules();
			return ret;
		}
	}
}