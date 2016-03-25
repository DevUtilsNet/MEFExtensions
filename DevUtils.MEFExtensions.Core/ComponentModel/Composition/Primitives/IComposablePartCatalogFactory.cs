using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives
{
	/// <summary> Interface for composable part catalog factory. </summary>
	public interface IComposablePartCatalogFactory
	{
		/// <summary> Gets composable part catalog. </summary>
		///
		/// <param name="scopeFull"> The scope full. </param>
		///
		/// <returns> The composable part catalog. </returns>
		ComposablePartCatalog GetComposablePartCatalog(string scopeFull);
	}
}
