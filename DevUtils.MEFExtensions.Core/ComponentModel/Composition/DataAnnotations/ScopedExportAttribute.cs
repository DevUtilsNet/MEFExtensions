using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for scoped export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class ScopedExportAttribute
		 : ExportAttribute
	{
		/// <summary> Gets or sets the name of the scope. </summary>
		///
		/// <value> The name of the scope. </value>
		public static readonly ScopeName ScopeName = default(ScopeName);

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

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeName"> Gets or sets the name of the scope. </param>
		public ScopedExportAttribute(ScopeName scopeName)
		{
			ScopeFullName = scopeName.FullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeName">	  Gets or sets the name of the scope. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ScopedExportAttribute(ScopeName scopeName, Type contractType)
			: base(contractType)
		{
			ScopeFullName = scopeName.FullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeName">	  Gets or sets the name of the scope. </param>
		/// <param name="contractName"> Name of the contract. </param>
		public ScopedExportAttribute(ScopeName scopeName, string contractName)
			: base(contractName)
		{
			ScopeFullName = scopeName.FullName;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeName">	  Gets or sets the name of the scope. </param>
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ScopedExportAttribute(ScopeName scopeName, string contractName, Type contractType)
			: base(contractName, contractType)
		{
			ScopeFullName = scopeName.FullName;
		}
	}
}