using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	/// <summary> A guarded application catalog. </summary>
	public class GuardedApplicationCatalog
		: ComposablePartCatalog
		, ICompositionElement
	{
		private bool _isDisposed;

		private volatile AggregateCatalog _innerCatalog;

		private readonly object _thisLock = new object();

		private readonly ICompositionElement _definitionOrigin;

		private readonly ReflectionContext _reflectionContext;

		private AggregateCatalog InnerCatalog
		{
			get
			{
				if (_innerCatalog == null)
				{
					var flag = false;
					var thisLock = _thisLock;
					try
					{
						Monitor.Enter(thisLock, ref flag);
						if (_innerCatalog == null)
						{
							var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

							var list = new List<ComposablePartCatalog>
							{
								CreateCatalog(baseDirectory, "*.exe"),
								CreateCatalog(baseDirectory, "*.dll")
							};
							var relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
							if (!string.IsNullOrEmpty(relativeSearchPath))
							{
								var array = relativeSearchPath.Split(new[]
								{
									';'
								}, StringSplitOptions.RemoveEmptyEntries);

								for (var i = 0; i < array.Length; i++)
								{
									var path = array[i];
									var text = Path.Combine(baseDirectory, path);
									if (Directory.Exists(text))
									{
										list.Add(CreateCatalog(text, "*.dll"));
									}
								}
							}
							var innerCatalog = new AggregateCatalog(list);
							_innerCatalog = innerCatalog;
						}
					}
					finally
					{
						if (flag)
						{
							Monitor.Exit(thisLock);
						}
					}
				}
				return _innerCatalog;
			}
		}

		string ICompositionElement.DisplayName => GetDisplayName();

		ICompositionElement ICompositionElement.Origin => null;

		/// <summary> Default constructor. </summary>
		public GuardedApplicationCatalog()
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="definitionOrigin"> The definition origin. </param>
		public GuardedApplicationCatalog(ICompositionElement definitionOrigin)
		{
			_definitionOrigin = definitionOrigin;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="reflectionContext"> Context for the reflection. </param>
		public GuardedApplicationCatalog(ReflectionContext reflectionContext)
		{
			_reflectionContext = reflectionContext;
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="reflectionContext"> Context for the reflection. </param>
		/// <param name="definitionOrigin">  The definition origin. </param>
		public GuardedApplicationCatalog(ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			_reflectionContext = reflectionContext;
			_definitionOrigin = definitionOrigin;
		}

		internal ComposablePartCatalog CreateCatalog(string location, string pattern)
		{
			ComposablePartCatalog ret;
			if (_reflectionContext != null)
			{
				ret = _definitionOrigin == null ? 
					new GuardedDirectoryCatalog(location, pattern, _reflectionContext) : 
					new GuardedDirectoryCatalog(location, pattern, _reflectionContext, _definitionOrigin);
			}
			else
			{
				ret = _definitionOrigin == null ? 
					new GuardedDirectoryCatalog(location, pattern) : 
					new GuardedDirectoryCatalog(location, pattern, _definitionOrigin);
			}
			return ret;
		}

		/// <summary> Releases the unmanaged resources used by the
		/// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> and
		/// optionally releases the managed resources. </summary>
		///
		/// <param name="disposing">	true to release both managed and unmanaged resources; false to
		/// 													release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!_isDisposed)
				{
					IDisposable disposable;
					var flag = false;
					var thisLock = _thisLock;
					try
					{

						Monitor.Enter(thisLock, ref flag);
						disposable = _innerCatalog;
						_innerCatalog = null;
						_isDisposed = true;
					}
					finally
					{
						if (flag)
						{
							Monitor.Exit(thisLock);
						}
					}
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary> Returns an enumerator that iterates through the catalog. </summary>
		///
		/// <returns> An enumerator that can be used to iterate through the catalog. </returns>
		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			ThrowIfDisposed();
			return InnerCatalog.GetEnumerator();
		}

		/// <summary> Gets a list of export definitions that match the constraint defined by the specified
		/// <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> object. </summary>
		///
		/// <param name="definition">	The conditions of the
		/// 													<see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" />
		/// 													objects to be returned. </param>
		///
		/// <returns> A collection of <see cref="T:System.Tuple`2" /> containing the
		/// <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects and
		/// their associated
		/// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects
		/// for objects that match the constraint specified by <paramref name="definition" />. </returns>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			ThrowIfDisposed();
			return InnerCatalog.GetExports(definition);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		private string GetDisplayName()
		{
			return string.Format(
				CultureInfo.CurrentCulture, "{0} (Path=\"{1}\") (PrivateProbingPath=\"{2}\")",
				GetType().Name, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath);
		}

		/// <summary> Returns a string that represents the current object. </summary>
		///
		/// <returns> A string that represents the current object. </returns>
		public override string ToString()
		{
			return GetDisplayName();
		}
	}
}
