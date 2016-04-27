using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for service host module export. </summary>
	public class ServiceHostModuleExportAttribute
		: ApplicationModuleExportAttribute
	{
		/// <summary> Default constructor. </summary>
		public ServiceHostModuleExportAttribute()
			: base(new ScopeName(ServiceHostExportAttribute.ScopeName))
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected ServiceHostModuleExportAttribute(ScopeName descendantScopeName)
			: base(ServiceHostExportAttribute.ScopeName / descendantScopeName)
		{
		}
	}
}