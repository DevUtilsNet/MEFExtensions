using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
	/// <summary> A guarded directory catalog. This class cannot be inherited. </summary>
	public sealed class GuardedDirectoryCatalog
				: ComposablePartCatalog
				, INotifyComposablePartCatalogChanged
				, ICompositionElement
	{
		private readonly object _thisLock = new object();
		private readonly ReflectionContext _reflectionContext;
		private readonly ICompositionElement _definitionOrigin;

		private string _searchPattern;
		private volatile bool _isDisposed;
		private IQueryable<ComposablePartDefinition> _partsQuery;
		private ComposablePartCatalogCollection _catalogCollection;
		private Dictionary<string, AssemblyCatalog> _assemblyCatalogs;

		/// <summary>
		///     Path passed into the constructor of <see cref="DirectoryCatalog"/>.
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		///     Translated absolute path of the path passed into the constructor of <see cref="DirectoryCatalog"/>. 
		/// </summary>
		public string FullPath { get; private set; }

		/// <summary>
		///     Gets the display name of the directory catalog.
		/// </summary>
		/// <value>
		///     A <see cref="String"/> containing a human-readable display name of the <see cref="DirectoryCatalog"/>.
		/// </value>
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		string ICompositionElement.DisplayName => GetDisplayName();

		/// <summary>
		///   SearchPattern passed into the constructor of <see cref="DirectoryCatalog"/>, or the default *.dll.
		/// </summary>
		public string SearchPattern => _searchPattern;

		/// <summary>
		///     Gets the composition element from which the directory catalog originated.
		/// </summary>
		/// <value>
		///     This property always returns <see langword="null"/>.
		/// </value>
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		ICompositionElement ICompositionElement.Origin => _definitionOrigin;

		/// <summary>
		///     Set of files that have currently been loaded into the catalog.
		/// </summary>
		public ReadOnlyCollection<string> LoadedFiles { get; private set; }

		/// <summary>
		///     Gets the part definitions of the directory catalog.
		/// </summary>
		/// <value>
		///     A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the 
		///     <see cref="DirectoryCatalog"/>.
		/// </value>
		/// <exception cref="ObjectDisposedException">
		///     The <see cref="DirectoryCatalog"/> has been disposed of.
		/// </exception>
		public override IQueryable<ComposablePartDefinition> Parts
		{
			get
			{
				ThrowIfDisposed();
				return _partsQuery;
			}
		}

		/// <summary>
		///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
		///     in the given directory path.
		///     
		///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
		///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
		/// </summary>
		/// <param name="path">
		///     Path to the directory to scan for assemblies to add to the catalog.
		///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
		/// </param>
		/// <exception cref="ArgumentException">
		///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
		///     contains one or more implementation-specific invalid characters.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="path"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
		///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
		///     be less than 260 characters. 
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		///     The caller does not have the required permission. 
		/// </exception>
		public GuardedDirectoryCatalog(string path)
				: this(path, "*.dll")
		{
		}

		/// <summary>
		///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given searchPattern 
		///     over the files in the given directory path.
		///     
		///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
		///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
		/// </summary>
		/// <param name="path">
		///     Path to the directory to scan for assemblies to add to the catalog.
		///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
		/// </param>
		/// <param name="searchPattern">
		///     Any valid searchPattern that <see cref="Directory.GetFiles(string, string)"/> will accept.
		/// </param>
		/// <exception cref="ArgumentException">
		///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
		///     contains one or more implementation-specific invalid characters. Or <paramref name="searchPattern"/> 
		///     does not contain a valid pattern. 
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="path"/> is <see langword="null"/> or <paramref name="searchPattern"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="DirectoryNotFoundException">
		///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
		/// </exception>
		/// <exception cref="PathTooLongException">
		///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
		///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
		///     be less than 260 characters. 
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		///     The caller does not have the required permission. 
		/// </exception>
		public GuardedDirectoryCatalog(string path, string searchPattern)
		{
			_definitionOrigin = this;
			Initialize(path, searchPattern);
		}

		/// <summary> Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given
		/// searchPattern over the files in the given directory path.
		/// 
		/// Possible exceptions that can be thrown are any that
		/// <see cref="Directory.GetFiles(string, string)"/> or
		/// <see cref="Assembly.Load(AssemblyName)"/> can throw. </summary>
		///
		/// <param name="path">						 	Path to the directory to scan for assemblies to add to the
		/// 																catalog. The path needs to be absolute or relative to
		/// 																<see cref="AppDomain.BaseDirectory"/> </param>
		/// <param name="searchPattern">	 	Any valid searchPattern that
		/// 																<see cref="Directory.GetFiles(string, string)"/> will accept. </param>
		/// <param name="definitionOrigin"> The definition origin. </param>
		public GuardedDirectoryCatalog(string path, string searchPattern, ICompositionElement definitionOrigin)
		{
			_definitionOrigin = definitionOrigin;
			Initialize(path, searchPattern);
		}

		/// <summary> Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given
		/// searchPattern over the files in the given directory path.
		/// 
		/// Possible exceptions that can be thrown are any that
		/// <see cref="Directory.GetFiles(string, string)"/> or
		/// <see cref="Assembly.Load(AssemblyName)"/> can throw. </summary>
		///
		/// <param name="path">								Path to the directory to scan for assemblies to add to the
		/// 																	catalog. The path needs to be absolute or relative to
		/// 																	<see cref="AppDomain.BaseDirectory"/> </param>
		/// <param name="searchPattern">			Any valid searchPattern that
		/// 																	<see cref="Directory.GetFiles(string, string)"/> will accept. </param>
		/// <param name="reflectionContext"> Context for the reflection. </param>
		public GuardedDirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext)
		{
			_definitionOrigin = this;
			_reflectionContext = reflectionContext;
			Initialize(path, searchPattern);
		}

		/// <summary> Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given
		/// searchPattern over the files in the given directory path.
		/// 
		/// Possible exceptions that can be thrown are any that
		/// <see cref="Directory.GetFiles(string, string)"/> or
		/// <see cref="Assembly.Load(AssemblyName)"/> can throw. </summary>
		///
		/// <param name="path">								Path to the directory to scan for assemblies to add to the
		/// 																	catalog. The path needs to be absolute or relative to
		/// 																	<see cref="AppDomain.BaseDirectory"/> </param>
		/// <param name="searchPattern">			Any valid searchPattern that
		/// 																	<see cref="Directory.GetFiles(string, string)"/> will accept. </param>
		/// <param name="reflectionContext"> Context for the reflection. </param>
		/// <param name="definitionOrigin">  The definition origin. </param>
		public GuardedDirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			_reflectionContext = reflectionContext;
			_definitionOrigin = definitionOrigin;
			Initialize(path, searchPattern);
		}

		/// <summary>
		/// Notify when the contents of the Catalog has changed.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>
		/// Notify when the contents of the Catalog has changing.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (!_isDisposed)
					{
						ComposablePartCatalogCollection catalogs = null;

						lock (_thisLock)
						{
							try
							{
								if (!_isDisposed)
								{
									catalogs = _catalogCollection;
									_catalogCollection = null;
									_assemblyCatalogs = null;
									_isDisposed = true;
								}
							}
							finally
							{
								if (catalogs != null)
								{
									catalogs.Dispose();
								}
							}
						}
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		///     Returns the export definitions that match the constraint defined by the specified definition.
		/// </summary>
		/// <param name="definition">
		///     The <see cref="ImportDefinition"/> that defines the conditions of the 
		///     <see cref="ExportDefinition"/> objects to return.
		/// </param>
		/// <returns>
		///     An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1, T2}"/> containing the 
		///     <see cref="ExportDefinition"/> objects and their associated 
		///     <see cref="ComposablePartDefinition"/> for objects that match the constraint defined 
		///     by <paramref name="definition"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///     <paramref name="definition"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///     The <see cref="DirectoryCatalog"/> has been disposed of.
		/// </exception>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			ThrowIfDisposed();

			return _catalogCollection.SelectMany(catalog => catalog.GetExports(definition));
		}

		/// <summary>
		///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changed"/> event.
		/// </summary>
		/// <param name="e">
		///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
		/// </param>
		private void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changedEvent = Changed;
			if (changedEvent != null)
			{
				changedEvent(this, e);
			}
		}

		/// <summary>
		///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changing"/> event.
		/// </summary>
		/// <param name="e">
		///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
		/// </param>
		private void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changingEvent = Changing;
			if (changingEvent != null)
			{
				changingEvent(this, e);
			}
		}

		/// <summary>
		///     Refreshes the <see cref="ComposablePartDefinition"/>s with the latest files in the directory that match
		///     the searchPattern. If any files have been added they will be added to the catalog and if any files were
		///     removed they will be removed from the catalog. For files that have been removed keep in mind that the 
		///     assembly cannot be unloaded from the process so <see cref="ComposablePartDefinition"/>s for those files
		///     will simply be removed from the catalog.
		/// 
		///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
		///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
		/// </summary>
		/// <exception cref="DirectoryNotFoundException">
		///     The specified <paramref /> has been removed since object construction.
		/// </exception>
		public void Refresh()
		{
			ThrowIfDisposed();

			ComposablePartDefinition[] addedDefinitions;
			ComposablePartDefinition[] removedDefinitions;

			while (true)
			{
				var afterFiles = GetFiles();

				object changeReferenceObject;
				string[] beforeFiles;
				lock (_thisLock)
				{
					changeReferenceObject = LoadedFiles;
					beforeFiles = LoadedFiles.ToArray();
				}

				List<Tuple<string, AssemblyCatalog>> catalogsToAdd;
				List<Tuple<string, AssemblyCatalog>> catalogsToRemove;
				DiffChanges(beforeFiles, afterFiles, out catalogsToAdd, out catalogsToRemove);

				// Don't go any further if there's no work to do
				if (catalogsToAdd.Count == 0 && catalogsToRemove.Count == 0)
				{
					return;
				}

				// Notify listeners to give them a preview before completeting the changes
				addedDefinitions = catalogsToAdd
						.SelectMany(cat => cat.Item2.Parts)
						.ToArray();

				removedDefinitions = catalogsToRemove
						.SelectMany(cat => cat.Item2.Parts)
						.ToArray();

				using (var atomicComposition = new AtomicComposition())
				{
					var changingArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, atomicComposition);
					OnChanging(changingArgs);

					// if the change went through then write the catalog changes
					lock (_thisLock)
					{
						if (changeReferenceObject != LoadedFiles)
						{
							// Someone updated the list while we were diffing so we need to try the diff again
							continue;
						}

						foreach (var catalogToAdd in catalogsToAdd)
						{
							_assemblyCatalogs.Add(catalogToAdd.Item1, catalogToAdd.Item2);
							_catalogCollection.Add(catalogToAdd.Item2);
						}

						foreach (var catalogToRemove in catalogsToRemove)
						{
							_assemblyCatalogs.Remove(catalogToRemove.Item1);
							_catalogCollection.Remove(catalogToRemove.Item2);
						}

						_partsQuery = _catalogCollection.AsQueryable().SelectMany(catalog => catalog.Parts);
						LoadedFiles = new ReadOnlyCollection<string>(afterFiles);

						// Lastly complete any changes added to the atomicComposition during the change event
						atomicComposition.Complete();

						// Break out of the while(true)
						break;
					} // WriteLock
				} // AtomicComposition
			}   // while (true)

			var changedArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, null);
			OnChanged(changedArgs);
		}

		/// <summary>
		///     Returns a string representation of the directory catalog.
		/// </summary>
		/// <returns>
		///     A <see cref="String"/> containing the string representation of the <see cref="DirectoryCatalog"/>.
		/// </returns>
		public override string ToString()
		{
			return GetDisplayName();
		}

		private AssemblyCatalog CreateAssemblyCatalogGuarded(string assemblyFilePath)
		{
			try
			{
				var ret = _reflectionContext != null ? new AssemblyCatalog(assemblyFilePath, _reflectionContext, this) : new AssemblyCatalog(assemblyFilePath, this);
				if (ret.Parts.Any())
				{
					return ret;
				}
			}
			catch (FileNotFoundException)
			{   // Files should always exists but don't blow up here if they don't
			}
			catch (FileLoadException)
			{   // File was found but could not be loaded
			}
			catch (BadImageFormatException)
			{   // Dlls that contain native code are not loaded, but do not invalidate the Directory
			}
			catch (ReflectionTypeLoadException)
			{   // Dlls that have missing Managed dependencies are not loaded, but do not invalidate the Directory 
			}

			return null;
		}

		private void DiffChanges(string[] beforeFiles, string[] afterFiles,
				out List<Tuple<string, AssemblyCatalog>> catalogsToAdd,
				out List<Tuple<string, AssemblyCatalog>> catalogsToRemove)
		{
			catalogsToAdd = new List<Tuple<string, AssemblyCatalog>>();
			catalogsToRemove = new List<Tuple<string, AssemblyCatalog>>();

			var filesToAdd = afterFiles.Except(beforeFiles);
			foreach (var file in filesToAdd)
			{
				var catalog = CreateAssemblyCatalogGuarded(file);

				if (catalog != null)
				{
					catalogsToAdd.Add(new Tuple<string, AssemblyCatalog>(file, catalog));
				}
			}

			IEnumerable<string> filesToRemove = beforeFiles.Except(afterFiles);
			lock (_thisLock)
			{
				foreach (var file in filesToRemove)
				{
					AssemblyCatalog catalog;
					if (_assemblyCatalogs.TryGetValue(file, out catalog))
					{
						catalogsToRemove.Add(new Tuple<string, AssemblyCatalog>(file, catalog));
					}
				}
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture,
													"{0} (Path=\"{1}\")",   // NOLOC
													GetType().Name,
													Path);
		}

		private string[] GetFiles()
		{
			return Directory.GetFiles(FullPath, _searchPattern);
		}

		private static string GetFullPath(string path)
		{
			if (!System.IO.Path.IsPathRooted(path))
			{
				path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
			}

			return System.IO.Path.GetFullPath(path);
		}

		private void Initialize(string path, string searchPattern)
		{
			Path = path;
			FullPath = GetFullPath(path);
			_searchPattern = searchPattern;
			_assemblyCatalogs = new Dictionary<string, AssemblyCatalog>();
			_catalogCollection = new ComposablePartCatalogCollection(null);

			LoadedFiles = new ReadOnlyCollection<string>(GetFiles());

			foreach (var file in LoadedFiles)
			{
				var assemblyCatalog = CreateAssemblyCatalogGuarded(file);

				if (assemblyCatalog != null)
				{
					_assemblyCatalogs.Add(file, assemblyCatalog);
					_catalogCollection.Add(assemblyCatalog);
				}
			}
			_partsQuery = _catalogCollection.AsQueryable().SelectMany(catalog => catalog.Parts);
		}

		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}
	}
}