using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace igiagbin
{
    public class DocumentStoreHolder
    {
        // Use Lazy<IDocumentStore> to initialize the document store lazily. 
        // This ensures that it is created only once - when first accessing the public `Store` property.
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Store => store.Value;

        private static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                // Define the cluster node URLs (required)
                Urls = new[] { Environment.GetEnvironmentVariable("RAVENDB_URL") ?? throw new Exception("RavenDB Url is required"), 
                           /*some additional nodes of this cluster*/ },

                // Set conventions as necessary (optional)
                Conventions =
            {
                MaxNumberOfRequestsPerSession = 10,
                UseOptimisticConcurrency = true
            },

                // Define a default database (optional)
                Database = Environment.GetEnvironmentVariable("RAVENDB_DATABASE") ?? throw new Exception("RavenDB Database is required"),

                // Initialize the Document Store
            }.Initialize();

            return store;
        }
    }
}
