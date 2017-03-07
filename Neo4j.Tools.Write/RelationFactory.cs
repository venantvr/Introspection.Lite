using System;
using System.Collections.Generic;
using Neo4jClient.Extension.Cypher;

namespace Neo4j.Tools.Write
{
    public class RelationFactory<TFromEndPoint, TToEndPoint>
    {
        public RelationFactory(Func<TFromEndPoint, IEnumerable<TToEndPoint>> targetEntities, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationBuilder)
        {
            // properties of source : TFromEndPoint (c)
            // properties of target : TToEndPoint (TargetEntities)
            TargetEntities = targetEntities;
            RelationBuilderWithParameters = relationBuilder;
        }

        public RelationFactory(Func<TFromEndPoint, TToEndPoint> targetEntity, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationBuilder)
        {
            // properties of source : TFromEndPoint (c)
            // properties of target : TToEndPoint (TargetEntities)
            TargetEntities = c => new[] { targetEntity.Invoke(c) };
            RelationBuilderWithParameters = relationBuilder;
        }

        //public RelationFactory(Func<TFromEndPoint, IEnumerable<TToEndPoint>> targetEntities, Func<string, string, BaseRelationship> relationBuilder)
        //{
        //    // properties of source : TFromEndPoint (c)
        //    // properties of target : TToEndPoint (TargetEntities)
        //    TargetEntities = targetEntities;
        //    RelationBuilder = relationBuilder;
        //}

        //public RelationFactory(Func<TFromEndPoint, TToEndPoint> targetEntity, Func<string, string, BaseRelationship> relationBuilder)
        //{
        //    // properties of source : TFromEndPoint (c)
        //    // properties of target : TToEndPoint (TargetEntities)
        //    TargetEntities = c => new[] { targetEntity.Invoke(c) };
        //    RelationBuilder = relationBuilder;
        //}

        public Func<TFromEndPoint, IEnumerable<TToEndPoint>> TargetEntities { get; }

        //public Func<string, string, BaseRelationship> RelationBuilder { get; }

        public Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> RelationBuilderWithParameters { get; }
    }
}