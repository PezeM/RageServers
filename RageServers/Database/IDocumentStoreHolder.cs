using Raven.Client.Documents;

namespace RageServers.Database
{
    public interface IDocumentStoreHolder
    {
        IDocumentStore Store { get; }
    }
}
