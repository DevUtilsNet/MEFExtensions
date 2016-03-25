using System.ComponentModel.Composition.Primitives;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> A data annotations composable part catalog factory. This class cannot be
	/// inherited. </summary>
	public sealed class DataAnnotationsComposablePartCatalogFactory
			: IComposablePartCatalogFactory
	{
		private readonly ComposablePartCatalog _rootCatalog;

		/// <summary> Constructor. </summary>
		///
		/// <param name="rootCatalog"> The root catalog. </param>
		public DataAnnotationsComposablePartCatalogFactory(ComposablePartCatalog rootCatalog)
		{
			_rootCatalog = rootCatalog;
		}

		/// <summary> Gets composable part catalog. </summary>
		///
		/// <param name="scopeFullName"> The scope full. </param>
		///
		/// <returns> The composable part catalog. </returns>
		public ComposablePartCatalog GetComposablePartCatalog(string scopeFullName)
		{
			var ret = new DataAnnotationsCatalog(_rootCatalog, scopeFullName);
			return ret;
		}
	}
}
