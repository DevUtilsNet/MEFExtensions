using System;
using System.ComponentModel.Composition;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for application export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class ApplicationExportAttribute
		: ScopedExportAttribute
	{
		/// <summary>
		/// Name of the scope.
		/// </summary>
		public const string ApplicationScopeName = "Application";

		/// <summary> Default constructor. </summary>
		public ApplicationExportAttribute()
			: base(ApplicationScopeName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public ApplicationExportAttribute(Type contractType)
			: base(ApplicationScopeName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public ApplicationExportAttribute(string contractName)
			: base(ApplicationScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ApplicationExportAttribute(string contractName, Type contractType)
			: base(ApplicationScopeName, contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected ApplicationExportAttribute(string descendantScopeName, string contractName)
			: base(CombainScopes(ApplicationScopeName, descendantScopeName), contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ApplicationExportAttribute(string descendantScopeName, string contractName, Type contractType)
			: base(CombainScopes(ApplicationScopeName, descendantScopeName), contractName, contractType)
		{
		}
	}
}