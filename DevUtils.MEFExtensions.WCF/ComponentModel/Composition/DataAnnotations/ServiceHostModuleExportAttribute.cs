using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for service host module export. </summary>
	public class ServiceHostModuleExportAttribute
		: ApplicationModuleExportAttribute
	{
		/// <summary> Default constructor. </summary>
		public ServiceHostModuleExportAttribute()
			: base(ServiceHostExportAttribute.ScopeName)
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected ServiceHostModuleExportAttribute(string descendantScopeName)
			: base(CombainScopes(ServiceHostExportAttribute.ScopeName, descendantScopeName))
		{
		}
	}
}