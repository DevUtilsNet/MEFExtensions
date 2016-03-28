using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for service host export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class ServiceHostExportAttribute
		: ApplicationExportAttribute
	{
		/// <summary>
		/// Name of the scope.
		/// </summary>
		public const string ServiceHostScopeName = "ServiceHost";

		/// <summary> Default constructor. </summary>
		public ServiceHostExportAttribute()
			: base(ServiceHostScopeName, null, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public ServiceHostExportAttribute(Type contractType)
			: base(ServiceHostScopeName, null, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public ServiceHostExportAttribute(string contractName)
			: base(ServiceHostScopeName, contractName, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ServiceHostExportAttribute(string contractName, Type contractType)
			: base(ServiceHostScopeName, contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected ServiceHostExportAttribute(string descendantScopeName, string contractName)
			: base(CombainScopes(ServiceHostScopeName, descendantScopeName), contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ServiceHostExportAttribute(string descendantScopeName, string contractName, Type contractType)
			: base(CombainScopes(ServiceHostScopeName, descendantScopeName), contractName, contractType)
		{
		}
	}
}