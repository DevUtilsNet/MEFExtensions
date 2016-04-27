using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

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
		public new static readonly string ScopeName = "Application";

		/// <summary> Default constructor. </summary>
		public ApplicationExportAttribute()
			: base(new ScopeName(ScopeName))
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public ApplicationExportAttribute(Type contractType)
			: base(new ScopeName(ScopeName), contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public ApplicationExportAttribute(string contractName)
			: base(new ScopeName(ScopeName), contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public ApplicationExportAttribute(string contractName, Type contractType)
			: base(new ScopeName(ScopeName), contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected ApplicationExportAttribute(ScopeName descendantScopeName)
			: base(ScopeName / descendantScopeName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ApplicationExportAttribute(ScopeName descendantScopeName, Type contractType)
			: base(ScopeName / descendantScopeName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected ApplicationExportAttribute(ScopeName descendantScopeName, string contractName)
			: base(ScopeName / descendantScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected ApplicationExportAttribute(ScopeName descendantScopeName, string contractName, Type contractType)
			: base(ScopeName / descendantScopeName, contractName, contractType)
		{
		}
	}
}