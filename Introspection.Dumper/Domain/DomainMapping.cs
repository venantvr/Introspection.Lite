using Introspection.Dumper.Dto;
using Neo4j.Tools.Write.ContractResolver;
using Neo4j.Tools.Write.Interfaces;
using Neo4jClient.Extension.Cypher;
using Newtonsoft.Json.Serialization;

namespace Introspection.Dumper.Domain
{
    public class DomainMapping : IDomainMapping
    {
        private DefaultContractResolver _jsonContractResolver;

        private FluentConfig DomainFluentMapping => FluentConfig.Config(new CypherExtensionContext
                                                                        {
                                                                            JsonContractResolver = JsonContractResolver
                                                                        });

        public DefaultContractResolver JsonContractResolver => _jsonContractResolver ?? (_jsonContractResolver = new PreserveCaseContractResolver());

        public IDomainMapping WithContractResolver(DefaultContractResolver resolver)
        {
            _jsonContractResolver = resolver;

            return this;
        }

        public void Fluent()
        {
            DomainFluentMapping
                .With<SimpleMethod>("Method")
                .Match(x => x.BusinessId)
                .Merge(x => x.BusinessId)
                .MergeOnMatchOrCreate(x => x.BusinessId)
                .MergeOnMatchOrCreate(p => p.FullName)
                .MergeOnMatchOrCreate(p => p.Name)
                .Set();

            DomainFluentMapping
                .With<SimpleType>("Class")
                .Match(x => x.BusinessId)
                .Merge(x => x.BusinessId)
                .MergeOnMatchOrCreate(x => x.BusinessId)
                .MergeOnMatchOrCreate(p => p.FullName)
                .MergeOnMatchOrCreate(p => p.Name)
                .Set();

            DomainFluentMapping
                .With<SimpleNamespace>("Namespace")
                .Match(x => x.BusinessId)
                .Merge(x => x.BusinessId)
                .MergeOnMatchOrCreate(x => x.BusinessId)
                .MergeOnMatchOrCreate(p => p.FullName)
                .Set();
        }
    }
}