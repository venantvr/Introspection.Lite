using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Introspection.Dumper.Domain;
using Introspection.Dumper.Dto;
using Introspection.Dumper.Relations;
using Neo4j.Tools.Write;
using Neo4j.Tools.Write.ContractResolver;
using Neo4j.Tools.Write.Hash;
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
                    Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> callsRelationshipFactory = (fromEntity, from, toEntity, to) => new CallsRelationship(@from, @to);
                    Func<SimpleMethod, string, SimpleType, string, BaseRelationship> typeRelationshipFactory = (fromEntity, from, toEntity, to) => new TypeRelationship(@from, @to);
                    Func<SimpleType, string, SimpleNamespace, string, BaseRelationship> namespaceRelationshipFactory = (fromEntity, from, toEntity, to) => new NamespaceRelationship(@from, @to);

                    fluent
                        .EncypherWithParameters(entities, entity => entity.Calls, callsRelationshipFactory)
                        .EncypherWithParameters(entities, entity => entity.Type, typeRelationshipFactory)
                        .EncypherWithParameters(entities.Select(e => e.Type), entity => entity.Namespace, namespaceRelationshipFactory);

                    Console.WriteLine(fluent.DebugQueryText);

                    fluent.ExecuteWithoutResults();
                }
            }
        }
    }
}