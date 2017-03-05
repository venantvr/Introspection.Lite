using Introspection.Neo4j.Write.Interfaces;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json.Serialization;

namespace Introspection.Neo4j.Write
{
    public class ProxyCypherFluentBuilder<TDomainMapping, TContractResolver, THashProcessor>
        where TDomainMapping : IDomainMapping, new()
        where TContractResolver : DefaultContractResolver, new()
        where THashProcessor : IHashProcessor, new()
    {
        private readonly IGraphClient _client;

        public ProxyCypherFluentBuilder(IGraphClient client)
        {
            _client = client;
        }

        public IProxyCypherFluent Build()
        {
            if (_client.IsConnected == false) _client.Connect();

            var domainMapping = new TDomainMapping().WithContractResolver(new TContractResolver());

            return new ProxyCypherFluent(new CypherFluentQuery(_client))
                .WithDomainMapping(domainMapping)
                .WithHashProcessor(new THashProcessor());
        }
    }
}