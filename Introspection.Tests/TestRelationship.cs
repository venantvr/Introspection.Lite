using Neo4jClient.Extension.Cypher;

namespace Introspection.Tests
{
    public class TestRelationship<TFromEndPoint, TToEndPoint> : BaseRelationship
    {
        public TestRelationship(string key) : base(key)
        {
        }

        public TestRelationship(string fromKey, string toKey) : base(fromKey, toKey)
        {
        }

        public TestRelationship(TFromEndPoint @from, string fromKey, TToEndPoint @to, string toKey) : base(fromKey, toKey)
        {
            From = from;
            To = to;
        }

        public TestRelationship(string key, string fromKey, string toKey) : base(key, fromKey, toKey)
        {
        }

        public TFromEndPoint From { get; set; }
        public TToEndPoint To { get; set; }
    }
}