using Neo4jClient.Extension.Cypher;

namespace Introspection.Dumper.Relations
{
    public class TypeRelationship : BaseRelationship
    {
        public TypeRelationship(string key) : base(key)
        {
        }

        public TypeRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public TypeRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}