using System;
using System.ComponentModel.Composition;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.DataAnnotations;

namespace DevUtils.MEFExtensions.WCF.ComponentModel.Composition.DataAnnotations
{
	/// <summary> Attribute for instance export. </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class InstanceExportAttribute
		: ApplicationExportAttribute
	{
		/// <summary> Name of the instance scope. </summary>
		public const string InstanceScopeName = "Instance";

		/// <summary> Default constructor. </summary>
		public InstanceExportAttribute()
			: base(InstanceScopeName, (string)null)
		{
		}
	}
}