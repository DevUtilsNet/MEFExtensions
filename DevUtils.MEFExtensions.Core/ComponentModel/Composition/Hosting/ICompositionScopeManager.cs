using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	/// <summary>
	/// Interface for composition scope manager.
	/// </summary>
	public interface ICompositionScopeManager 
		: IDisposable
	{
		/// <summary> Gets the name of the scope. </summary>
		///
		/// <value> The name of the scope. </value>
		string ScopeName { get; }

		/// <summary> Gets the name of the scope full. </summary>
		///
		/// <value> The name of the scope full. </value>
		string ScopeFullName { get; }

		/// <summary> Gets the catalog. </summary>
		///
		/// <value> The catalog. </value>
		ComposablePartCatalog Catalog { get; }

		/// <summary> Gets the provider. </summary>
		///
		/// <value> The provider. </value>
		CompositionContainer Container { get; }

		/// <summary> Creates composition scope manager. </summary>
		///
		/// <param name="scope"> The scope. </param>
		///
		/// <returns> The new composition scope manager. </returns>
		ICompositionScopeManager CreateCompositionScopeManager(string scope);

		/// <summary> Creates composition scope manager. </summary>
		///
		/// <param name="scope">   The scope. </param>
		/// <param name="customCatalog"> The catalog. </param>
		///
		/// <returns> The new composition scope manager. </returns>
		ICompositionScopeManager CreateCompositionScopeManager(string scope, ComposablePartCatalog customCatalog);
	}
}