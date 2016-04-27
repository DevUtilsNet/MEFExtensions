using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives
{
	/// <summary> Interface for composable part catalog factory. </summary>
	public interface IComposablePartCatalogFactory
	{
		/// <summary> Gets composable part catalog. </summary>
		///
		/// <param name="scopeName"> Name of the full scope. </param>
		///
		/// <returns> The composable part catalog. </returns>
		ComposablePartCatalog GetComposablePartCatalog(ScopeName scopeName);
	}
}