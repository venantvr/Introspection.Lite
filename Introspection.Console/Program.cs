using System;
using System.Collections.Generic;
using System.Linq;
using Introspection.Analysis.Models.Introspection.Analysis.Models;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts;
using Introspection.Dumper;
using Introspection.Dumper.Dto;

namespace Introspection.Console
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pathToAssembly = args[0];

            using (var resolver = new AssemblyResolver())
            {
                var enumerable = resolver.Resolve(pathToAssembly);
                var assemblyTuple = BfCache.GetTupleDictionary(enumerable);

                using (var bfCache = new BfCache(assemblyTuple))
                {
                    Func<BfMethod, bool> predicate = m => /*m.IsConstructor == false && m.IsPropertyGetter == false && m.IsPropertySetter == false && m.IsProperty == false &&*/ m.FullName.StartsWith(@"System") == false;

                    var simpleMethods = bfCache.Methods // .Skip(2).Take(1)
                        .Where(predicate)
                        .Select(m => new SimpleMethod
                                     {
                                         BusinessId = m.MethodId,
                                         Name = m.Name,
                                         FullName = m.FullName,
                                         Type = new SimpleType
                                                {
                                                    BusinessId = m.Type.TypeId,
                                                    Name = m.Type.Name,
                                                    FullName = m.Type.FullName,
                                                    Namespace = new SimpleNamespace
                                                                {
                                                                    BusinessId = m.Type.Namespace.NamespaceId,
                                                                    FullName = m.Type.Namespace.Name
                                                                }
                                                },
                                         Calls = m.Calls.Where(predicate).Select(c => new SimpleMethod
                                                                                      {
                                                                                          BusinessId = c.MethodId,
                                                                                          Name = c.Name,
                                                                                          FullName = c.FullName,
                                                                                          Type = new SimpleType
                                                                                                 {
                                                                                                     BusinessId = c.Type.TypeId,
                                                                                                     Name = c.Type.Name,
                                                                                                     FullName = c.Type.FullName,
                                                                                                     Namespace = new SimpleNamespace
                                                                                                                 {
                                                                                                                     BusinessId = c.Type.Namespace.NamespaceId,
                                                                                                                     FullName = c.Type.Namespace.Name
                                                                                                                 }
                                                                                                 },
                                                                                          Calls = new List<SimpleMethod>()
                                                                                      })
                                     });

                    new ObjectDumper().DumpAsNeo4J(simpleMethods.ToList());
                }
            }
        }
    }
}