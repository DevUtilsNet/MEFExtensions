using System;
using System.ComponentModel.Composition;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for scoped export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class ScopedExportAttribute
		 : ExportAttribute
	{
		/// <summary> Gets or sets the name of the scope full. </summary>
		///
		/// <value> The name of the scope full. </value>
		public string ScopeFullName { get; set; }

		/// <summary> Default constructor. </summary>
		public ScopedExportAttribute()
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public ScopedExportAttribute(Type contractType)
			: base(contractType)
		{
			
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeFullName"> The name of the scope full. </param>
		public ScopedExportAttribute(string scopeFullName)
		{
			ScopeFullName = scopeFullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeFullName"> The name of the scope full. </param>
		/// <param name="contractType">  Type of the contract. </param>
		public ScopedExportAttribute(string scopeFullName, Type contractType)
			: base(contractType)
		{
			ScopeFullName = scopeFullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeFullName"> The name of the scope full. </param>
		/// <param name="contractName">  Name of the contract. </param>
		public ScopedExportAttribute(string scopeFullName, string contractName)
			: base(contractName)
		{
			ScopeFullName = scopeFullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeFullName"> The name of the scope full. </param>
		/// <param name="contractName">  Name of the contract. </param>
		/// <param name="contractType">  Type of the contract. </param>
		public ScopedExportAttribute(string scopeFullName, string contractName, Type contractType)
			: base(contractName, contractType)
		{
			ScopeFullName = scopeFullName;
		}

		/// <summary> Combain scopes. </summary>
		///
		/// <param name="scopes"> A variable-length parameters list containing scopes. </param>
		///
		/// <returns> A string. </returns>
		public static string CombainScopes(params string[] scopes)
		{
			var ret = string.Join("/", scopes);
			return ret;
		}
	}
}