# Introspection.Lite

Goal: to push a complex graph of entities into a Neo4j database by matching objects with nodes, and navigation properties with relations.

This project is based on the official Neo4jClient API and the Neo4jClient.Extension extension library.

This library allows to transfer, with a fluent syntax, objects and their dependencies into a Neo4j database.

Example:

    using (var client = new GraphClient(new Uri(neo4jServerUrl), neo4jUserName, neo4jUserPassword))
    {
        using (var fluent = new ProxyCypherFluentBuilder<DomainMapping, PreserveCaseContractResolver, Md5HashProcessor>(client).Build())
        {
            Func<string, string, BaseRelationship> namespaceRelationshipFactory = (from, to) => new NamespaceRelationship(@from, to);

            fluent.Encypher(entities.Select(e => e.Type), entity => entity.Namespace, namespaceRelationshipFactory);

            fluent.ExecuteWithoutResults();
        }
    }