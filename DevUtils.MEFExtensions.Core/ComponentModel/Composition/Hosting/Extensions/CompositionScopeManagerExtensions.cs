using System.ComponentModel.Composition.Primitives;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting.Extensions
{
	/// <summary> A composition scope manager extensions. </summary>
	public static class CompositionScopeManagerExtensions
	{
		/// <summary> Creates composition scope manager. </summary>
		///
		/// <param name="manager">					 The manager to act on. </param>
		/// <param name="scopeName">				 The scope. </param>
		/// <param name="customCatalog">		 The custom catalog. </param>
		/// <param name="cascadeDelete">		 true to cascade delete. </param>
		/// <param name="initializeModules"> true to initialize modules. </param>
		///
		/// <returns> The new composition scope manager. </returns>
		public static ICompositionScopeManager CreateCompositionScopeManager(
			this ICompositionScopeManager manager,
			ScopeName scopeName,
			ComposablePartCatalog customCatalog = null,
			bool cascadeDelete = true,
			bool initializeModules = true)
		{
			var ret = manager.CreateCompositionScopeManager(scopeName.Name, customCatalog, cascadeDelete, initializeModules);
			return ret;
		}
	}
}