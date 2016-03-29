using System;
using System.ComponentModel.Composition;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for instance export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class InstanceExportAttribute
		: ServiceHostExportAttribute
	{
		/// <summary> Name of the instance scope. </summary>
		public new const string ScopeName = "Instance";

		/// <summary> Default constructor. </summary>
		public InstanceExportAttribute()
			: base(ScopeName, null, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public InstanceExportAttribute(Type contractType)
			: base(ScopeName, null, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public InstanceExportAttribute(string contractName)
			: base(ScopeName, contractName, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public InstanceExportAttribute(string contractName, Type contractType)
			: base(ScopeName, contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected InstanceExportAttribute(string descendantScopeName, string contractName)
			: base(CombainScopes(ScopeName, descendantScopeName), contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected InstanceExportAttribute(string descendantScopeName, string contractName, Type contractType)
			: base(CombainScopes(ScopeName, descendantScopeName), contractName, contractType)
		{
		}
	}
}