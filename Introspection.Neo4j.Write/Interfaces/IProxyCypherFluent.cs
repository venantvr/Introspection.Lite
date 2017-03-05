using System;
using System.Collections.Generic;
using Neo4jClient.Extension.Cypher;

namespace Introspection.Neo4j.Write.Interfaces
{
    public interface IProxyCypherFluent : IDisposable
    {
        IHashProcessor HashProcessor { get; }
        Dictionary<string, int> HashDone { get; }
        //ICypherFluentQuery CypherFluentQuery { get; }
        string DebugQueryText { get; }

        IProxyCypherFluent WithHashProcessor(IHashProcessor hashProcessor);

        IProxyCypherFluent WithDomainMapping(IDomainMapping domainMapping);

        string HashIdentifier<TFromEndPoint>(TFromEndPoint entity, string identifier) where TFromEndPoint : class;

        IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(TFromEndPoint entity,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(IReadOnlyCollection<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent Encypher<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, IEnumerable<TToEndPoint>> @to, Func<string, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent Encypher<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, TToEndPoint> @to, Func<string, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent EncypherWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, IEnumerable<TToEndPoint>> @to, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent EncypherWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, TToEndPoint> @to, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class;

        void ExecuteWithoutResults();

        IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(IReadOnlyCollection<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(TFromEndPoint entity,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;

        IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class;
    }
}