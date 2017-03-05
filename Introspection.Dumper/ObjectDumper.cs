using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Introspection.Dumper.Domain;
using Introspection.Dumper.Dto;
using Introspection.Dumper.Relations;
using Introspection.Neo4j.Write;
using Introspection.Neo4j.Write.ContractResolver;
using Introspection.Neo4j.Write.Hash;
using Neo4jClient;
using Neo4jClient.Extension.Cypher;

namespace Introspection.Dumper
{
    public class ObjectDumper
    {
        public void DumpAsNeo4J(List<SimpleMethod> entities)
        {
            // ReSharper disable once InconsistentNaming
            var neo4jServerUrl = ConfigurationManager.AppSettings["Neo4jServerUrl"];
            // ReSharper disable once InconsistentNaming
            var neo4jUserName = ConfigurationManager.AppSettings["Neo4jUserName"];
            // ReSharper disable once InconsistentNaming
            var neo4jUserPassword = ConfigurationManager.AppSettings["Neo4jUserPassword"];

            using (var client = new GraphClient(new Uri(neo4jServerUrl), neo4jUserName, neo4jUserPassword))
            {
                using (var fluent = new ProxyCypherFluentBuilder<DomainMapping, PreserveCaseContractResolver, Md5HashProcessor>(client).Build())
                {
                    Func<string, string, BaseRelationship> callsRelationshipFactory = (from, to) => new CallsRelationship(@from, @to);
                    Func<string, string, BaseRelationship> typeRelationshipFactory = (from, to) => new TypeRelationship(@from, @to);
                    Func<string, string, BaseRelationship> namespaceRelationshipFactory = (from, to) => new NamespaceRelationship(@from, @to);

                    fluent
                        //.Encypher(entities, entity => entity.Calls, callsRelationshipFactory)
                        //.Encypher(entities, entity => entity.Type, typeRelationshipFactory);
                        .Encypher(entities.Select(e => e.Type), entity => entity.Namespace, namespaceRelationshipFactory);

                    Console.WriteLine(fluent.DebugQueryText);

                    fluent.ExecuteWithoutResults();
                }
            }
        }
    }
}