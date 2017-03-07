namespace Neo4j.Tools.Write.Interfaces
{
    public interface IHashProcessor
    {
        string GetHash<T>(T instance);
    }
}