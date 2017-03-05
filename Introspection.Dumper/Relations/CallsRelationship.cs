using Neo4jClient.Extension.Cypher;

namespace Introspection.Dumper.Relations
{
    public class CallsRelationship : BaseRelationship
    {
        public CallsRelationship(string key) : base(key)
        {
        }

        public CallsRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public CallsRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }
    }
}