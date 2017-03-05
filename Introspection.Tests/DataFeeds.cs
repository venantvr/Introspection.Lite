using System.Collections.Generic;
using Introspection.Dumper.Dto;

namespace Introspection.Tests
{
    public class DataFeeds
    {
        protected static List<SimpleMethod> ListOfMethodsThatLinksToAnotherSingleOne
        {
            get
            {
                var entities = new List<SimpleMethod>();

                for (var i = 0; i < 10; i++)
                {
                    entities.Add(new SimpleMethod
                                 {
                                     BusinessId = i.ToString(),
                                     FullName = i.ToString(),
                                     Calls = new List<SimpleMethod>
                                             {
                                                 new SimpleMethod
                                                 {
                                                     BusinessId = $"other_{i}",
                                                     FullName = $"other_{i}"
                                                 }
                                             }
                                 });
                }

                return entities;
            }
        }

        protected static List<SimpleMethod> ListOfMethodsThatLinksToAnotherOneButWithADuplicate
        {
            get
            {
                var entities = new List<SimpleMethod>();

                for (var i = 0; i < 10; i++)
                {
                    entities.Add(new SimpleMethod
                                 {
                                     BusinessId = i.ToString(),
                                     FullName = i.ToString(),
                                     Calls = new List<SimpleMethod>
                                             {
                                                 new SimpleMethod
                                                 {
                                                     BusinessId = i == 2 ? "9" : $"other_{i}",
                                                     FullName = i == 2 ? "9" : $"other_{i}"
                                                 }
                                             }
                                 });
                }

                return entities;
            }
        }

        protected static List<SimpleMethod> OneMethodThatLinksToTwoOthersWithTypesAndNamespaces
        {
            get
            {
                var entities = new List<SimpleMethod>();

                for (var i = 0; i < 1; i++)
                {
                    var simpleType = new SimpleType
                                     {
                                         BusinessId = $"type_{i}",
                                         FullName = $"type_{i}",
                                         Namespace = new SimpleNamespace
                                                     {
                                                         BusinessId = $"namespace_{i + 1}"
                                                     }
                                     };

                    entities.Add(new SimpleMethod
                                 {
                                     BusinessId = i.ToString(),
                                     FullName = i.ToString(),
                                     Calls = new List<SimpleMethod>
                                             {
                                                 new SimpleMethod
                                                 {
                                                     BusinessId = $"other_{i}",
                                                     FullName = $"other_{i}",
                                                     Type = simpleType
                                                 },
                                                 new SimpleMethod
                                                 {
                                                     BusinessId = $"other_{i + 1}",
                                                     FullName = $"other_{i + 1}",
                                                     Type = simpleType
                                                 }
                                             },
                                     Type = simpleType
                                 });
                }

                return entities;
            }
        }
    }
}