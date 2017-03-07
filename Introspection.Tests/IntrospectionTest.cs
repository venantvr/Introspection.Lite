using System;
using System.Collections.Generic;
using System.Linq;
using Introspection.Analysis.Models.Introspection.Analysis.Models;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts;
using Introspection.Dumper.Dto;
using Introspection.Dumper.Relations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4j.Tools.Write;
using Neo4jClient.Extension.Cypher;

namespace Introspection.Tests
{
    [TestClass]
    public class IntrospectionTest : IntrospectionTestBase
    {
        [TestMethod]
        public void DumpAsNeo4J_01()
        {
            foreach (var item in ListOfMethodsThatLinksToAnotherSingleOne)
            {
                Fluent.HashIdentifier(item, null);
            }

            Assert.AreEqual(Fluent.HashDone.Count, 10);
        }

        [TestMethod]
        public void DumpAsNeo4J_02()
        {
            var entities = ListOfMethodsThatLinksToAnotherSingleOne.Union(ListOfMethodsThatLinksToAnotherSingleOne);

            foreach (var item in entities)
            {
                Fluent.HashIdentifier(item, null);
            }

            Assert.AreEqual(Fluent.HashDone.Count, 10);
        }

        [TestMethod]
        public void DumpAsNeo4J_11()
        {
            Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> callsRelationshipFactory = (fromEntity, from, toEntity, to) => new CallsRelationship(@from, @to);

            Fluent.CypherObject(ListOfMethodsThatLinksToAnotherSingleOne, new RelationFactory<SimpleMethod, SimpleMethod>(entity => new List<SimpleMethod>() /*.Calls*/, callsRelationshipFactory));

            Console.WriteLine(Fluent.DebugQueryText);

            var query = Fluent.DebugQueryText.Split(StringSeparators, StringSplitOptions.None).Where(l => l.Length > 0);

            Assert.AreEqual(query.Count(), 10);
        }

        [TestMethod]
        public void DumpAsNeo4J_12()
        {
            Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> callsRelationshipFactory = (fromEntity, from, toEntity, to) => new CallsRelationship(@from, @to);

            Fluent.CypherObject(ListOfMethodsThatLinksToAnotherSingleOne, new RelationFactory<SimpleMethod, SimpleMethod>(entity => entity.Calls, callsRelationshipFactory));

            Console.WriteLine(Fluent.DebugQueryText);

            var query = Fluent.DebugQueryText.Split(StringSeparators, StringSplitOptions.None).Where(l => l.Length > 0);

            Assert.AreEqual(query.Count(), 30);
        }

        [TestMethod]
        public void DumpAsNeo4J_13()
        {
            Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> callsRelationshipFactory = (fromEntity, from, toEntity, to) => new CallsRelationship(@from, @to);

            Fluent.CypherObject(ListOfMethodsThatLinksToAnotherOneButWithADuplicate, new RelationFactory<SimpleMethod, SimpleMethod>(entity => entity.Calls, callsRelationshipFactory));

            Console.WriteLine(Fluent.DebugQueryText);

            var query = Fluent.DebugQueryText.Split(StringSeparators, StringSplitOptions.None).Where(l => l.Length > 0);

            Assert.AreEqual(query.Count(), Relations(10) + Nodes(2 * 10 - 1)); // 28
        }

        [TestMethod]
        public void DumpAsNeo4J_14()
        {
            var entities = OneMethodThatLinksToTwoOthersWithTypesAndNamespaces;

            Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> callsRelationshipFactory = (fromEntity, from, toEntity, to) => new CallsRelationship(@from, @to);
            Func<SimpleMethod, string, SimpleType, string, BaseRelationship> typeRelationshipFactory = (fromEntity, from, toEntity, to) => new TypeRelationship(@from, @to);
            Func<SimpleType, string, SimpleNamespace, string, BaseRelationship> namespaceRelationshipFactory = (fromEntity, from, toEntity, to) => new NamespaceRelationship(@from, @to);

            Fluent
                .CypherObject(entities, new RelationFactory<SimpleMethod, SimpleMethod>(entity => entity.Calls, callsRelationshipFactory))
                .CypherObject(entities, new RelationFactory<SimpleMethod, SimpleType>(entity => entity.Type, typeRelationshipFactory))
                .CypherObject(entities.Select(e => e.Type), new RelationFactory<SimpleType, SimpleNamespace>(entity => entity.Namespace, namespaceRelationshipFactory));

            Console.WriteLine(Fluent.DebugQueryText);

            var query = Fluent.DebugQueryText.Split(StringSeparators, StringSplitOptions.None).Where(l => l.Length > 0);

            Assert.AreEqual(query.Count(), 9); // Relations(3) + Nodes(3));
        }

        [TestMethod]
        public void DumpAsNeo4J_15()
        {
            var pathToAssembly = @"C:\Users\Rénald\Source\Repos\SomeRepo\SomeApplication\bin\Debug\Application.exe";

            using (var resolver = new AssemblyResolver())
            {
                var enumerable = resolver.Resolve(pathToAssembly);
                var assemblyTuple = BfCache.GetTupleDictionary(enumerable);

                using (var bfCache = new BfCache(assemblyTuple))
                {
                    Assert.AreEqual(bfCache.Methods.Count(), 624);
                }
            }
        }

        [TestMethod]
        public void DumpAsNeo4J_16()
        {
            Func<SimpleMethod, string, SimpleMethod, string, BaseRelationship> testRelationshipFactory = (fromEntity, from, toEntity, to) => new TestRelationship<SimpleMethod, SimpleMethod>(fromEntity, @from, toEntity, @to); // { From = fromEntity, To = toEntity };

            Fluent.CypherObjectWithParameters(ListOfMethodsThatLinksToAnotherOneButWithADuplicate, new RelationFactory<SimpleMethod, SimpleMethod>(entity => entity.Calls, testRelationshipFactory));

            Console.WriteLine(Fluent.DebugQueryText);

            var query = Fluent.DebugQueryText.Split(StringSeparators, StringSplitOptions.None).Where(l => l.Length > 0);

            Assert.AreEqual(query.Count(), Relations(10) + Nodes(2 * 10 - 1)); // 28
        }
    }
}