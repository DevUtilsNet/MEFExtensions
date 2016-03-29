using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting;
using DevUtils.MEFExtensions.Web.ComponentModel.Composition.DataAnnotations;
using DevUtils.MEFExtensions.Web.Extensions;

namespace DevUtils.MEFExtensions.Web.MVC
{
	/// <summary> A MEF dependency resolver. This class cannot be inherited. </summary>
	public sealed class MefDependencyResolver
			: IDependencyResolver
	{
		private static readonly object Key = typeof(MefDependencyResolver);

		private readonly ICompositionScopeManager _manager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager"> The manager. </param>
		public MefDependencyResolver(ICompositionScopeManager manager)
		{
			_manager = manager;
		}

		private CompositionContainer GetContainer()
		{
			var ctx = HttpContext.Current;
			var manager = ctx.GetData<ICompositionScopeManager>(Key);
			if (manager == null)
			{
				manager = _manager.CreateCompositionScopeManager(HttpContextExportAttribute.ScopeName);

				ctx.SetData(Key, manager);

				ctx.AddOnRequestCompleted(OnRequestCompleted);
			}

			var ret = manager.Container;
			return ret;
		}

		private static void OnRequestCompleted(HttpContext httpContext)
		{
			var container = httpContext.GetData<ICompositionScopeManager>(Key);
			container?.Dispose();
		}

		#region Implementation of IDependencyResolver

		/// <summary>
		/// Resolves singly registered services that support arbitrary object creation.
		/// </summary>
		/// <returns>
		/// The requested service or object.
		/// </returns>
		/// <param name="serviceType">The type of the requested service or object.</param>
		public object GetService(Type serviceType)
		{
			var ret = GetServices(serviceType).SingleOrDefault();
			return ret;
		}

		/// <summary>
		/// Resolves multiply registered services.
		/// </summary>
		/// <returns>
		/// The requested services.
		/// </returns>
		/// <param name="serviceType">The type of the requested services.</param>
		public IEnumerable<object> GetServices(Type serviceType)
		{
			var container = GetContainer();

			var lazyRet = container.GetExports(serviceType, null, string.Empty);
			var ret = lazyRet.Select(s => s.Value);
			return ret;
		}

		#endregion
	}
}