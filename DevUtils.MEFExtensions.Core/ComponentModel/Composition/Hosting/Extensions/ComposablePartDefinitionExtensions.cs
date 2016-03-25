using System.Collections;
using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions
{
	static class ComposablePartDefinitionExtensions
	{
		public static IEnumerable GetExportMetadataValueWithKey(this ComposablePartDefinition part, string key)
		{
			foreach (var item in part.ExportDefinitions)
			{
				object value;
				if (item.Metadata.TryGetValue(key, out value))
				{
					yield return value;
				}
			}
		}
	}
}