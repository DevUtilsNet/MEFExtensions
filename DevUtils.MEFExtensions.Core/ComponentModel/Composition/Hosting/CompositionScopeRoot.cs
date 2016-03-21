using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
    /// <summary> A composition scope root. </summary>
    public sealed class CompositionScopeRoot
    {
        private readonly IComposablePartCatalogFactory _catalogFactory;

        /// <summary> Default constructor. </summary>
        public CompositionScopeRoot()
            : this(new ApplicationCatalog())
        {
        }

        /// <summary> Constructor. </summary>
        ///
        /// <param name="rootCatalog"> The root catalog. </param>
        public CompositionScopeRoot(
            ComposablePartCatalog rootCatalog)
            : this(new DataAnnotationsComposablePartCatalogFactory(rootCatalog))
        {
            
        }

        /// <summary> Constructor. </summary>
        ///
        /// <param name="catalogFactory"> The catalog factory. </param>
        public CompositionScopeRoot(IComposablePartCatalogFactory catalogFactory)
        {
            _catalogFactory = catalogFactory;
        }

        /// <summary> Creates root scope. </summary>
        ///
        /// <returns> The new root scope. </returns>
        public ICompositionScopeManager CreateRootScopeManager()
        {
            var ret = new CompositionScopeManager(_catalogFactory);
            return ret;
        }
    }
}