namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for application module export. </summary>
	public class ApplicationModuleExportAttribute
		: ModuleExportAttribute
	{
		/// <summary> Default constructor. </summary>
		public ApplicationModuleExportAttribute()
			: base(ApplicationExportAttribute.ScopeName)
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected ApplicationModuleExportAttribute(string descendantScopeName)
			: base(CombainScopes(ApplicationExportAttribute.ScopeName, descendantScopeName))
		{
		}
	}
}