using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
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
		private readonly Dictionary<string, ComposablePartCatalog> _cachedCatalogs = new Dictionary<string, ComposablePartCatalog>();

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
			scopeFullName = scopeFullName ?? string.Empty;

			ComposablePartCatalog ret;
			lock (_cachedCatalogs)
			{
				if (!_cachedCatalogs.TryGetValue(scopeFullName, out ret))
				{
					ret = new DataAnnotationsCatalog(_rootCatalog, scopeFullName);
					_cachedCatalogs[scopeFullName] = ret;
				}
			}

			return ret;
		}
	}
}
