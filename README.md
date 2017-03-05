# Introspection.Lite

Goal: to be able to push a complex graph of entities into a Neo4j database by matching objects with nodes, and navigation properties with relations.

This project is based on the official *Neo4jClient* API and the *Neo4jClient.Extension* extension library.

This library allows to transfer, with a fluent syntax, objects and their dependencies into a Neo4j database.

Example:

Imagine you built by introspection a list of all classes and namespaces embedded in your assemblies. Each namespace contains a set of classes. For persisting this network of dependencies, you should be able to write it as it figures below.

    using (var client = new GraphClient(new Uri(neo4jServerUrl), neo4jUserName, neo4jUserPassword))
    {
        using (var fluent = new ProxyCypherFluentBuilder<DomainMapping, PreserveCaseContractResolver, Md5HashProcessor>(client).Build())
        {
            Func<string, string, BaseRelationship> namespaceRelationshipFactory = (from, to) => new NamespaceRelationship(@from, to);

            fluent.Encypher(entities.Select(e => e.Type), entity => entity.Namespace, namespaceRelationshipFactory);

            fluent.ExecuteWithoutResults();
        }
    }
	
The *fluent* variable is the class built upon the *Neo4jClient.Extension* library. You have to specify the *DomainMapping* object that figures how your entities will map to the nodes.

You will then specify the *Newtonsoft Contract Resolver*, used to define how C# objects and properties will be serialized into JSON. 

Finally, you need to specify the *Hash Processor* that will help you to define what criteria are needed to resolve an identity for your nodes, to be able to merge similar objects into the graph.

The relationship is defined through an expression builder, and the *Encypher* method will do the magic.

**TODO**: put all that stuff into Nuget repositories, one for the introspection stuff, one for the Neo4j stuff...