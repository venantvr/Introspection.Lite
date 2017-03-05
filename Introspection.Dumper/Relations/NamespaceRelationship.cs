using Neo4jClient.Extension.Cypher;

namespace Introspection.Dumper.Relations
{
    public class NamespaceRelationship : BaseRelationship
    {
        public NamespaceRelationship(string key) : base(key)
        {
        }

        public NamespaceRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public NamespaceRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}