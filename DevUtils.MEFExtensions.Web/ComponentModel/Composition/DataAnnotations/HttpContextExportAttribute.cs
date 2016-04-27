using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Web.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for HTTP context export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
	public class HttpContextExportAttribute
		: AnyAllExportAttribute
	{
		/// <summary>
		/// Name of the scope.
		/// </summary>
		public new static readonly string ScopeName = "_HttpContext";

		/// <summary> Default constructor. </summary>
		public HttpContextExportAttribute()
			: base(new ScopeName(ScopeName), null, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractType"> Type of the contract. </param>
		public HttpContextExportAttribute(Type contractType)
			: base(new ScopeName(ScopeName), null, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		public HttpContextExportAttribute(string contractName)
			: base(new ScopeName(ScopeName), contractName, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="contractName"> Name of the contract. </param>
		/// <param name="contractType"> Type of the contract. </param>
		public HttpContextExportAttribute(string contractName, Type contractType)
			: base(new ScopeName(ScopeName), contractName, contractType)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		protected HttpContextExportAttribute(ScopeName descendantScopeName, string contractName)
			: base(descendantScopeName / ScopeName, contractName)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		/// <param name="contractName">				 Name of the contract. </param>
		/// <param name="contractType">				 Type of the contract. </param>
		protected HttpContextExportAttribute(ScopeName descendantScopeName, string contractName, Type contractType)
			: base(descendantScopeName / ScopeName, contractName, contractType)
		{
		}
	}
}