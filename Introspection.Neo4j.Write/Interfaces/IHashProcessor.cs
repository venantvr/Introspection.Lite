namespace Introspection.Neo4j.Write.Interfaces
{
    public interface IHashProcessor
    {
        string GetHash<T>(T instance);
    }
}