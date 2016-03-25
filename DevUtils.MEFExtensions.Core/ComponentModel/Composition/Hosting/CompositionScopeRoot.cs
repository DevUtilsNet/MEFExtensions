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
	}
}