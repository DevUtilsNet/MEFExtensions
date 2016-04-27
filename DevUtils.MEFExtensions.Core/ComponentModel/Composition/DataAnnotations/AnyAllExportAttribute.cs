using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for any all export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class AnyAllExportAttribute
		: ScopedExportAttribute
	{
		/// <summary>
		/// Name of the scope.
		/// </summary>
		public new static readonly string ScopeName = "**";

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected AnyAllExportAttribute(ScopeName descendantScopeName)
			: base(ScopeName / descendantScopeName)
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected AnyAllExportAttribute(ScopeName descendantScopeName, Type contractType)
			: base(ScopeName / descendantScopeName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected AnyAllExportAttribute(ScopeName descendantScopeName, string contractName)
			: base(ScopeName / descendantScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected AnyAllExportAttribute(ScopeName descendantScopeName, string contractName, Type contractType)
			: base(ScopeName / descendantScopeName, contractName, contractType)
		{
		}
	}
}