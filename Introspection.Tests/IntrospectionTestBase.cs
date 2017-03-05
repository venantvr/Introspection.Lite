using System;
using Introspection.Dumper.Domain;
using Introspection.Neo4j.Write;
using Introspection.Neo4j.Write.ContractResolver;
using Introspection.Neo4j.Write.Hash;
using Introspection.Neo4j.Write.Interfaces;
using Introspection.Tests.Mock;
using Neo4jClient.Cypher;

namespace Introspection.Tests
{
    public class IntrospectionTestBase : DataFeeds
    {
        protected readonly IProxyCypherFluent Fluent;
        protected readonly string[] StringSeparators = { "CREATE" };

        protected IntrospectionTestBase()
        {
            new DomainMapping().Fluent();

            var client = new MockGraphClient(new Uri("http://127.0.0.1:7474/db/data/"), "login", "password");

            Fluent = new ProxyCypherFluent(new CypherFluentQuery(client))
                .WithDomainMapping(new DomainMapping().WithContractResolver(new PreserveCaseContractResolver()))
                .WithHashProcessor(new BuiltInTypeHashProcessor());
        }

        protected int Relations(int nb)
        {
            return nb;
        }

        protected int Nodes(int nb)
        {
            return nb;
        }
    }
}