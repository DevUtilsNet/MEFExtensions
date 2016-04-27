using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for service host export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class ServiceHostExportAttribute
		: AnyAllExportAttribute
	{
		/// <summary>
		/// Name of the scope.
		/// </summary>
		public new static readonly string ScopeName = "_ServiceHost";

		/// <summary> Default constructor. </summary>
		public ServiceHostExportAttribute()
			: base(new ScopeName(ScopeName))
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public ServiceHostExportAttribute(Type contractType)
			: base(new ScopeName(ScopeName), contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public ServiceHostExportAttribute(string contractName)
			: base(new ScopeName(ScopeName), contractName, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ServiceHostExportAttribute(string contractName, Type contractType)
			: base(new ScopeName(ScopeName), contractName, contractType)
		{
		}

		/// <summary> Default constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected ServiceHostExportAttribute(ScopeName descendantScopeName)
			: base(ScopeName / descendantScopeName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ServiceHostExportAttribute(ScopeName descendantScopeName, Type contractType)
			: base(ScopeName / descendantScopeName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected ServiceHostExportAttribute(ScopeName descendantScopeName, string contractName)
			: base(ScopeName / descendantScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ServiceHostExportAttribute(ScopeName descendantScopeName, string contractName, Type contractType)
			: base(ScopeName / descendantScopeName, contractName, contractType)
		{
		}
	}
}