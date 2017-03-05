# Introspection.Lite

Objective: to push a complex graph of entities in a Neo4j database by matching objects and nodes, navigation properties and relations.

This project is based on the official Neo4jClient API and the Neo4jClient.Extension extension library.

This library allows to transfer with a fluent syntax objects and their dependencies to a Neo4j database.

Example:

    using (var client = new GraphClient(new Uri(neo4jServerUrl), neo4jUserName, neo4jUserPassword))
    {
        using (var fluent = new ProxyCypherFluentBuilder<DomainMapping, PreserveCaseContractResolver, Md5HashProcessor>(client).Build())
        {
            Func<string, string, BaseRelationship> namespaceRelationshipFactory = (from, to) => new NamespaceRelationship(@from, to);
    
            fluent
                .Encypher(entities.Select(e => e.Type), entity => entity.Namespace, namespaceRelationshipFactory);
    
            Console.WriteLine(fluent.DebugQueryText);
    
            fluent.ExecuteWithoutResults();
        }
    }