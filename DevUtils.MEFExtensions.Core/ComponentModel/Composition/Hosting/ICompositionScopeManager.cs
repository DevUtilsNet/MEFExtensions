﻿using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

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
		ScopeName ScopeFullName { get; }


		/// <summary> Gets the catalog. </summary>
		///
		/// <value> The catalog. </value>
		ComposablePartCatalog Catalog { get; }

		/// <summary> Gets the provider. </summary>
		///
		/// <value> The provider. </value>
		CompositionContainer Container { get; }

		/// <summary> Gets the manager for parent. </summary>
		///
		/// <value> The parent manager. </value>
		ICompositionScopeManager ParentManager { get; }

		/// <summary> Creates composition scope manager. </summary>
		///
		/// <param name="scope">						 The scope. </param>
		/// <param name="customCatalog">		 The catalog. </param>
		/// <param name="cascadeDelete">		 true to cascade delete. </param>
		/// <param name="initializeModules"> true to initialize modules. </param>
		///
		/// <returns> The new composition scope manager. </returns>
		ICompositionScopeManager CreateCompositionScopeManager(string scope, ComposablePartCatalog customCatalog = null, bool cascadeDelete = true, bool initializeModules = true);
	}
}