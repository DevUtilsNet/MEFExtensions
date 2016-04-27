using System.Collections.Generic;
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
		private readonly Dictionary<ScopeName, ComposablePartCatalog> _cachedCatalogs = new Dictionary<ScopeName, ComposablePartCatalog>();

		/// <summary> Constructor. </summary>
		///
		/// <param name="rootCatalog"> The root catalog. </param>
		public DataAnnotationsComposablePartCatalogFactory(ComposablePartCatalog rootCatalog)
		{
			_rootCatalog = rootCatalog;
		}

		/// <summary> Gets composable part catalog. </summary>
		///
		/// <param name="scopeName"> Name of the full scope. </param>
		///
		/// <returns> The composable part catalog. </returns>
		public ComposablePartCatalog GetComposablePartCatalog(ScopeName scopeName)
		{
			ComposablePartCatalog ret;
			lock (_cachedCatalogs)
			{
				if (!_cachedCatalogs.TryGetValue(scopeName, out ret))
				{
					ret = new DataAnnotationsCatalog(_rootCatalog, scopeName);
					_cachedCatalogs[scopeName] = ret;
				}
			}

			return ret;
		}
	}
}
