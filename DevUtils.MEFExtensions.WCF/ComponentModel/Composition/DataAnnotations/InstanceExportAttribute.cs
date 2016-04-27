using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for instance export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class InstanceExportAttribute
		: ServiceHostExportAttribute
	{
		/// <summary> Name of the instance scope. </summary>
		public new static readonly string ScopeName = "_Instance";

		/// <summary> Default constructor. </summary>
		public InstanceExportAttribute()
			: base(new ScopeName(ScopeName))
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public InstanceExportAttribute(Type contractType)
			: base(new ScopeName(ScopeName), contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public InstanceExportAttribute(string contractName)
			: base(new ScopeName(ScopeName), contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public InstanceExportAttribute(string contractName, Type contractType)
			: base(new ScopeName(ScopeName), contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected InstanceExportAttribute(ScopeName descendantScopeName, string contractName)
			: base(ScopeName / descendantScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected InstanceExportAttribute(ScopeName descendantScopeName, string contractName, Type contractType)
			: base(ScopeName / descendantScopeName, contractName, contractType)
		{
		}
	}
}