using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for module export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ModuleExportAttribute
		: ScopedExportAttribute
	{
		/// <summary> Constructor. </summary>
		///
		/// <param name="scopeFullName"> Name of the scope full. </param>
		public ModuleExportAttribute(string scopeFullName)
			: base(scopeFullName, scopeFullName, typeof(IScopeModule))
		{
		}
	}
}