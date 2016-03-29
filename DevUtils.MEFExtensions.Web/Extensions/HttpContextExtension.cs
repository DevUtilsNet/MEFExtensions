using System.Web;

namespace DevUtils.MEFExtensions.Web.Extensions
{
	/// <summary>
	/// A HTTP context extension.
	/// </summary>
	public static class HttpContextExtension
	{
		/// <summary>
		/// A HttpContext extension method that sets a data.
		/// </summary>
		/// <param name="context"> The context to act on. </param>
		/// <param name="key">		 The key. </param>
		/// <param name="data">		 The data. </param>
		public static void SetData(this HttpContext context, object key, object data)
		{
			context.Items[key] = data;
		}
		/// <summary>
		/// A HttpContext extension method that gets a data.
		/// </summary>
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="context"> The context to act on. </param>
		/// <param name="key">		 The key. </param>
		/// <returns>
		/// The data.
		/// </returns>
		public static T GetData<T>(this HttpContext context, object key) where T : class
		{
			return context.Items[key] as T;
		}
	}
}