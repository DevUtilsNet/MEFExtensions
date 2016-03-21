using System.ComponentModel.Composition.Primitives;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Hosting
{
    sealed class DataAnnotationsComposablePartCatalogFactory
        : IComposablePartCatalogFactory
    {
        private ComposablePartCatalog rootCatalog;

        public DataAnnotationsComposablePartCatalogFactory(ComposablePartCatalog rootCatalog)
        {
            this.rootCatalog = rootCatalog;
        }

        public ComposablePartCatalog GetComposablePartCatalog(string scopeFull)
        {
            throw new System.NotImplementedException();
        }
    }
}
