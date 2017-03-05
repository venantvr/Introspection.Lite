using System;
using System.Collections.Generic;
using System.Linq;
using Introspection.Neo4j.Write.Extensions;
using Introspection.Neo4j.Write.Hash;
using Introspection.Neo4j.Write.Interfaces;
using Neo4jClient.Cypher;
using Neo4jClient.Extension.Cypher;

namespace Introspection.Neo4j.Write
{
    public class ProxyCypherFluent : IProxyCypherFluent
    {
        private readonly SortedDictionary<ArtefactType, List<Action>> _todo = new SortedDictionary<ArtefactType, List<Action>> { { ArtefactType.Node, new List<Action>() }, { ArtefactType.Relation, new List<Action>() } };
        private readonly HashSet<string> _track = new HashSet<string>();
        private ICypherFluentQuery _cypherFluentQuery;
        private IDomainMapping _domainMapping;
        private int _fluentIndex;
        private IHashProcessor _hashProcessor;

        public ProxyCypherFluent(ICypherFluentQuery cypherFluentQuery)
        {
            _cypherFluentQuery = cypherFluentQuery;
        }

        private ICypherFluentQuery CypherFluentQuery
        {
            get
            {
                foreach (var item in _todo)
                {
                    item.Value.ForEach(i => i.Invoke());
                }

                _todo.Clear();

                return _cypherFluentQuery;
            }
        }

        public IHashProcessor HashProcessor => _hashProcessor ?? (_hashProcessor = new BuiltInTypeHashProcessor());

        public Dictionary<string, int> HashDone { get; } = new Dictionary<string, int>();

        public string DebugQueryText => CypherFluentQuery.Query.DebugQueryText;

        public void Dispose()
        {
        }

        public IProxyCypherFluent WithHashProcessor(IHashProcessor hashProcessor)
        {
            _hashProcessor = hashProcessor;
            return this;
        }

        public IProxyCypherFluent WithDomainMapping(IDomainMapping domainMapping)
        {
            _domainMapping = domainMapping;
            _domainMapping.Fluent();

            return this;
        }

        public string HashIdentifier<TFromEndPoint>(TFromEndPoint entity, string identifier) where TFromEndPoint : class
        {
            var newIdentifier = string.Empty;

            if (identifier == null)
            {
                var hash = _hashProcessor.GetHash(entity);

                var index = 0;

                if (HashDone.ContainsKey(hash))
                {
                    index = HashDone[hash];
                }
                else
                {
                    //_fluentIndex = _fluentIndex + 1;
                    HashDone.Add(hash, ++_fluentIndex);

                    index = _fluentIndex;
                }

                newIdentifier = $@"node_{index}";
            }
            else
            {
                newIdentifier = identifier;
            }

            return newIdentifier;
        }

        public IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(TFromEndPoint entity,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            return CypherObject(new[] { entity }, navigationProperties);
        }

        public IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            return CypherObject(entities.ToList().AsReadOnly(), navigationProperties);
        }

        public IProxyCypherFluent CypherObject<TFromEndPoint, TToEndPoint>(IReadOnlyCollection<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            entities.ForEach(item =>
                             {
                                 // ReSharper disable once AccessToDisposedClosure
                                 var fromKey = CreateEntity(item);

                                 navigationProperties.ForEach(navigationProperty =>
                                                              {
                                                                  var relationFactory = navigationProperty?.RelationBuilder;
                                                                  navigationProperty?.TargetEntities.Invoke(item).ForEach(c =>
                                                                                                                          {
                                                                                                                              // ReSharper disable once AccessToDisposedClosure
                                                                                                                              var toKey = CreateEntity(c);
                                                                                                                              // ReSharper disable once AccessToDisposedClosure
                                                                                                                              CreateRelationship(relationFactory.Invoke(fromKey, toKey));
                                                                                                                          }
                                                                      );
                                                              });
                             });

            return this;
        }

        public IProxyCypherFluent Encypher<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, IEnumerable<TToEndPoint>> to, Func<string, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            var expression = CypherObject(@from, new RelationFactory<TFromEndPoint, TToEndPoint>(to, relationshipFactory));
            return expression;
        }

        public IProxyCypherFluent Encypher<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, TToEndPoint> @to, Func<string, string, BaseRelationship> relationshipFactory)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            var expression = CypherObject(@from, new RelationFactory<TFromEndPoint, TToEndPoint>(to, relationshipFactory));
            return expression;
        }

        public IProxyCypherFluent EncypherWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, IEnumerable<TToEndPoint>> to, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationshipFactory) where TFromEndPoint : class where TToEndPoint : class
        {
            var expression = CypherObjectWithParameters(@from, new RelationFactory<TFromEndPoint, TToEndPoint>(to, relationshipFactory));
            return expression;
        }

        public IProxyCypherFluent EncypherWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> @from, Func<TFromEndPoint, TToEndPoint> to, Func<TFromEndPoint, string, TToEndPoint, string, BaseRelationship> relationshipFactory) where TFromEndPoint : class where TToEndPoint : class
        {
            var expression = CypherObjectWithParameters(@from, new RelationFactory<TFromEndPoint, TToEndPoint>(to, relationshipFactory));
            return expression;
        }

        public void ExecuteWithoutResults()
        {
            CypherFluentQuery.ExecuteWithoutResults();
        }

        public IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(IReadOnlyCollection<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            entities.ForEach(item =>
                             {
                                 // ReSharper disable once AccessToDisposedClosure
                                 var fromKey = CreateEntity(item);

                                 navigationProperties.ForEach(navigationProperty =>
                                                              {
                                                                  var relationFactory = navigationProperty?.RelationBuilderWithParameters;
                                                                  navigationProperty?.TargetEntities.Invoke(item).ForEach(c =>
                                                                                                                          {
                                                                                                                              // ReSharper disable once AccessToDisposedClosure
                                                                                                                              var toKey = CreateEntity(c);
                                                                                                                              // ReSharper disable once AccessToDisposedClosure
                                                                                                                              CreateRelationship(relationFactory.Invoke(item, fromKey, c, toKey));
                                                                                                                          }
                                                                      );
                                                              });
                             });

            return this;
        }

        public IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(TFromEndPoint entity,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            return CypherObjectWithParameters(new[] { entity }, navigationProperties);
        }

        public IProxyCypherFluent CypherObjectWithParameters<TFromEndPoint, TToEndPoint>(IEnumerable<TFromEndPoint> entities,
            params RelationFactory<TFromEndPoint, TToEndPoint>[] navigationProperties)
            where TFromEndPoint : class
            where TToEndPoint : class
        {
            return CypherObjectWithParameters(entities.ToList().AsReadOnly(), navigationProperties);
        }

        private string CreateEntity<TFromEndPoint>(TFromEndPoint entity, string identifier = null, List<CypherProperty> onCreateOverride = null, string preCql = "", string postCql = "")
            where TFromEndPoint : class
        {
            var newIdentifier = HashIdentifier(entity, identifier);

            if (!_track.Contains(newIdentifier))
            {
                _todo[ArtefactType.Node].Add(() => _cypherFluentQuery = _cypherFluentQuery.CreateEntity(entity, newIdentifier, onCreateOverride, preCql, postCql));
                _track.Add(newIdentifier);
            }

            return newIdentifier;
        }

        private void CreateRelationship<TRelation>(TRelation entity, CreateOptions options = null) where TRelation : BaseRelationship
        {
            var relation = $"{entity.FromKey} => {entity.ToKey}";

            if (_track.Contains(relation)) return;
            _todo[ArtefactType.Relation].Add(() => _cypherFluentQuery = _cypherFluentQuery.CreateRelationship(entity, options));
            _track.Add(relation);
        }
    }
}