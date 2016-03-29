using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;

namespace DevUtils.MEFExtensions.Web.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for HTTP context module export. </summary>
	public class HttpContextModuleExportAttribute
		: ApplicationModuleExportAttribute
	{
		/// <summary> Default constructor. </summary>
		public HttpContextModuleExportAttribute()
			: base(HttpContextExportAttribute.ScopeName)
		{
		}

		/// <summary> Specialised constructor for use only by derived class. </summary>
		///
		/// <param name="descendantScopeName"> Name of the descendant scope. </param>
		protected HttpContextModuleExportAttribute(string descendantScopeName)
			: base(CombainScopes(HttpContextExportAttribute.ScopeName, descendantScopeName))
		{
		}
	}
}