using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for instance module export. </summary>
	public class InstanceModuleExportAttribute
		: ServiceHostModuleExportAttribute
	{
		/// <summary> Default constructor. </summary>
		public InstanceModuleExportAttribute()
			: base(new ScopeName(InstanceExportAttribute.ScopeName))
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected InstanceModuleExportAttribute(ScopeName descendantScopeName)
			: base(InstanceExportAttribute.ScopeName / descendantScopeName)
		{
		}
	}
}