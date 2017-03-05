using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [Serializable]
    public class BfCache : IDisposable
    {
        private readonly Dictionary<string, BfAssembly> _assembliesDictionary = new Dictionary<string, BfAssembly>();

        private readonly Dictionary<string, BfAssembly> _dictionary = new Dictionary<string, BfAssembly>();
        private readonly Dictionary<string, ModelFile> _filesDictionary = new Dictionary<string, ModelFile>(StringComparer.InvariantCultureIgnoreCase);

        private readonly SortedDictionary<string, BfNamespace> _sortedDictionary = new SortedDictionary<string, BfNamespace>();

        private int _eventsCount;

        private BfCache()
        {
        }

        public BfCache(Dictionary<string, AssemblyTuple> nameToAssemblyTuple)
        {
            GetTypes(nameToAssemblyTuple);

            Assemblies.AddRange(new HashSet<BfAssembly>(_dictionary.Values));

            var index = 0;

            while (index < Types.Count)
            {
                var bfType = Types[index];

                if (bfType.Name != "<Module>")
                {
                    bfType.Populate();

                    Methods.AddRange(bfType.Methods);
                    Events.AddRange(bfType.Events);
                    Fields.AddRange(bfType.Fields);
                }

                ++index;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Types.Count; ++i)
            {
                Types[i].Methods.Distinct().ForEach(m => m.Populate());
            }

            Types.Distinct().ForEach(bfType => bfType.Commit());

            Types.Distinct().ForEach(bfType =>
                                     {
                                         bfType.Methods.ForEach(m => m.Dispose());
                                         bfType.Events.ForEach(m => m.Dispose());
                                         bfType.Fields.ForEach(m => m.Dispose());
                                     });

            Assemblies.Distinct().ForEach(bfAssembly => bfAssembly.Dispose());

            Delegates();
            ReIndex();
        }

        private AssemblyCollection Assemblies { get; } = new AssemblyCollection();

        private NamespaceCollection Namespaces { get; } = new NamespaceCollection();

        public TypeCollection Types { get; } = new TypeCollection();

        public MethodCollection Methods { get; } = new MethodCollection();

        private FieldCollection Fields { get; } = new FieldCollection();

        public EventCollection Events { get; } = new EventCollection();

        public void Dispose()
        {
            foreach (var bfType in Types.Where(type => type.Name.StartsWith("<PrivateImplementationDetails>")).ToList())
            {
                Types.Remove(bfType);

                bfType.Fields.ForEach(bfField =>
                                      {
                                          bfField.GotByMethods.ForEach(m => m.FieldGets.Remove(bfField));
                                          bfField.SetByMethods.ForEach(m => m.FieldGets.Remove(bfField));
                                          Fields.Remove(bfField);
                                      });

                bfType.Methods.ForEach(bfMethod =>
                                       {
                                           bfMethod.CalledBy.ForEach(m => m.Calls.Remove(bfMethod));
                                           Methods.Remove(bfMethod);
                                       });

                bfType.Events.ForEach(e => Events.Remove(e));
            }

            BaseCollection<BfType>.Dispose();
            BaseCollection<BfAssembly>.Dispose();
            BaseCollection<BfMethod>.Dispose();
            BaseCollection<BfNamespace>.Dispose();
            BaseCollection<BfField>.Dispose();
            BaseCollection<BfEvent>.Dispose();
        }

        private void GetTypes(Dictionary<string, AssemblyTuple> nameToAssemblyTuples)
        {
            var dictionary = new Dictionary<AssemblyDefinition, BfAssembly>();

            foreach (var assemblyTuple in nameToAssemblyTuples.Values.Where(a => !dictionary.ContainsKey(a.Assembly)))
            {
                var bfAssembly = new BfAssembly(assemblyTuple.Assembly, assemblyTuple.IsCoreAssembly, assemblyTuple.Directory);

                dictionary.Add(assemblyTuple.Assembly, bfAssembly);

                foreach (var moduleDefinition in assemblyTuple.Assembly.Modules)
                {
                    _assembliesDictionary.SafeAdd(moduleDefinition.Name, bfAssembly);

                    if (assemblyTuple.IsCoreAssembly)
                    {
                        moduleDefinition.Types.ForEach(t => CreateBfType(t, bfAssembly));
                    }
                }
            }

            nameToAssemblyTuples.Keys.ForEach(k => _dictionary.Add(k, dictionary[nameToAssemblyTuples[k].Assembly]));
        }

        public static Dictionary<string, AssemblyTuple> GetTupleDictionary(IEnumerable<AssemblyFileInfo> coreAssemblyFiles)
        {
            var dictionary = new Dictionary<string, AssemblyTuple>();
            var assemblyResolver = new DefaultAssemblyResolver();

            var assemblyFileInfos = coreAssemblyFiles as AssemblyFileInfo[] ?? coreAssemblyFiles.ToArray();

            assemblyFileInfos.ForEach(a => assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(a.Path)));

            foreach (var assemblyFileInfo in assemblyFileInfos)
            {
                var assembly = AssemblyDefinition.ReadAssembly(assemblyFileInfo.Path);

                dictionary.SafeAdd(assembly.Name.FullName, new AssemblyTuple(assembly)
                                                           {
                                                               IsCoreAssembly = true,
                                                               Directory = Path.GetDirectoryName(assemblyFileInfo.Path)
                                                           });
            }

            foreach (var name in new List<AssemblyTuple>(dictionary.Values)
                .SelectMany(a => a.Assembly.Modules)
                .SelectMany(a => a.AssemblyReferences))
            {
                try
                {
                    var assembly = assemblyResolver.Resolve(name);

                    var assemblyTuple = new AssemblyTuple(assembly)
                                        {
                                            Directory = "",
                                            IsCoreAssembly = false
                                        };

                    if (assembly.Name.FullName == name.FullName)
                    {
                        dictionary.SafeAdd(name.FullName, assemblyTuple);
                    }
                    else if (dictionary.ContainsKey(name.FullName) || dictionary.ContainsKey(assembly.Name.FullName))
                    {
                        dictionary.SafeAdd(name.FullName, dictionary[assembly.Name.FullName]);
                        dictionary.SafeAdd(assembly.Name.FullName, dictionary[name.FullName]);
                    }
                    else
                    {
                        dictionary.SafeAdd(name.FullName, assemblyTuple);
                        dictionary.SafeAdd(assembly.Name.FullName, assemblyTuple);
                    }
                }
                catch (FileNotFoundException)
                {
                }
            }

            return dictionary;
        }

        private void Delegates()
        {
            Types.Where(type => type.FullName == "System.Delegate").ForEach(Delegates);
        }

        private static void Delegates(BfType bfType)
        {
            bfType.IsDelegate = true;
            bfType.DerivedTypes.ForEach(Delegates);
        }

        internal ModelFile File(Instruction instruction)
        {
            var document = instruction.SequencePoint.Document;

            var file = !_filesDictionary.ContainsKey(document.Url) ? new ModelFile(document) : _filesDictionary[document.Url];

            _filesDictionary.SafeAdd(document.Url, file);

            return file;
        }

        internal BfMethod GetBfMethod(MethodReference methodReference)
        {
            BfMethod bfMethod = null;

            var bfAssembly = GetAssembly(methodReference, methodReference.DeclaringType.Scope as AssemblyNameReference);

            if (bfAssembly != null)
            {
                var key = BfMethod.GetSignature(methodReference);
                BfMethod method;
                bfAssembly.GetMethodsDictionary().TryGetValue(key, out method);

                if (method == null)
                {
                    var type = GetBfType(methodReference.DeclaringType);
                    var list = new List<MethodDefinition>();

                    if (type != null)
                    {
                        list.AddRange(type.GetTypeDefinition().Methods);

                        var methodDef = list.FirstOrDefault(m => key == BfMethod.GetSignature(m));

                        if (methodDef != null)
                        {
                            method = new BfMethod(this, methodDef, type);
                            Methods.Add(method);
                            type.Methods.Add(method);
                            bfAssembly.GetMethodsDictionary().Add(method.UniqueName, method);
                        }
                    }
                }

                bfMethod = method;
            }

            return bfMethod;
        }

        private BfAssembly GetAssembly(MemberReference reference, AssemblyNameReference scope)
        {
            BfAssembly bfAssembly = null;

            if (scope != null && _dictionary.ContainsKey(scope.FullName))
            {
                bfAssembly = _dictionary[scope.FullName];
            }
            else if (reference.DeclaringType.Scope is ModuleDefinition)
            {
                bfAssembly = _dictionary[((ModuleDefinition) reference.DeclaringType.Scope).Assembly.Name.FullName];
            }

            return bfAssembly;
        }

        internal BfField GetBfField(FieldReference fieldReference)
        {
            BfField bfField = null;

            var bfAssembly = GetAssembly(fieldReference, fieldReference.DeclaringType.Scope as AssemblyNameReference);

            if (bfAssembly != null)
            {
                var key = GetKey(fieldReference.DeclaringType);
                BfType type;
                bfAssembly.GetTypesDictionary().TryGetValue(key, out type);

                if (type != null)
                {
                    var field = type.Fields.FirstOrDefault(f => fieldReference.Name == f.Name);

                    if (field != null)
                    {
                        bfField = field;
                    }
                    else
                    {
                        var fieldDef = type.GetTypeDefinition().Fields.FirstOrDefault(f => f.Name == fieldReference.Name);

                        if (fieldDef != null)
                        {
                            var item = new BfField(this, fieldDef, type);
                            Fields.Add(item);
                            type.Fields.Add(item);
                            bfField = item;
                        }
                    }
                }
            }

            return bfField;
        }

        internal static string GetKey(TypeReference typeReference)
        {
            typeReference = GetTypeReference(typeReference);
            return Regex.Replace(typeReference.FullName, "<[^>]+>", "");
        }

        internal static TypeReference GetTypeReference(TypeReference typeReference)
        {
            while (typeReference is TypeSpecification)
                typeReference = ((TypeSpecification) typeReference).ElementType;
            return typeReference;
        }

        internal BfType GetBfType(TypeReference typeReference)
        {
            BfType bfType = null;

            if (typeReference == null)
            {
                bfType = null;
            }
            else
            {
                typeReference = GetTypeReference(typeReference);

                if (typeReference is GenericParameter)
                {
                    var bfAssembly = _dictionary.Values.Where(assembly =>
                                                              {
                                                                  if (assembly.Name == "mscorlib")
                                                                      return string.Compare(assembly.Version, "2.0.0.0", StringComparison.Ordinal) > -1;
                                                                  return false;
                                                              }).FirstOrDefault() ?? _dictionary.Values.FirstOrDefault(assembly => assembly.IsCoreAssembly);

                    // ReSharper disable once PossibleNullReferenceException
                    bfType = !bfAssembly.GetTypesDictionary().ContainsKey(typeReference.FullName)
                        ? CreateBfType(new TypeDefinition(typeReference.Name, typeReference.Namespace, TypeAttributes.Abstract, null), bfAssembly)
                        : bfAssembly.GetTypesDictionary()[typeReference.FullName];
                }
                else
                {
                    try
                    {
                        BfAssembly bfAssembly = null;

                        var scope = typeReference.Scope as AssemblyNameReference;

                        if (scope != null && _dictionary.ContainsKey(scope.FullName))
                            bfAssembly = _dictionary[scope.FullName];
                        else if (typeReference.Scope is ModuleDefinition && _dictionary.ContainsKey(((ModuleDefinition) typeReference.Scope).Assembly.Name.FullName))
                            bfAssembly = _dictionary[((ModuleDefinition) typeReference.Scope).Assembly.Name.FullName];
                        else if (typeReference.Scope is ModuleReference && _dictionary.ContainsKey(((ModuleReference) typeReference.Scope).Name))
                            bfAssembly = _assembliesDictionary[((ModuleReference) typeReference.Scope).Name];

                        BfType type = null;

                        if (bfAssembly != null)
                        {
                            bfAssembly.GetTypesDictionary().TryGetValue(GetKey(typeReference), out type);

                            if (type == null)
                            {
                                var typeDefinition = bfAssembly.GetAssemblyDefinition().Modules.SelectMany(m => m.Types).FirstOrDefault(p => typeReference.FullName == p.FullName);

                                if (typeDefinition != null)
                                {
                                    type = CreateBfType(typeDefinition, bfAssembly);
                                }
                            }
                        }

                        if (type != null)
                        {
                            bfType = type;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                }
            }

            return bfType;
        }

        internal TypeCollection GetTypeCollection(IGenericParameterProvider genericParameterProvider)
        {
            var typeCollection = new TypeCollection();
            foreach (var genericParameter in genericParameterProvider.GenericParameters)
            {
                typeCollection.Add(GetBfType(genericParameter));
                typeCollection.AddRange(GetTypeCollection(genericParameter));
            }
            return typeCollection;
        }

        private void ReIndex()
        {
            for (var index = 0; index < Assemblies.Count; ++index)
                Assemblies[index].AssemblyId = $"a{index}";
            for (var index = 0; index < Namespaces.Count; ++index)
                Namespaces[index].NamespaceId = $"n{index}";
            for (var index = 0; index < Types.Count; ++index)
                Types[index].TypeId = $"y{index}";
            for (var index = 0; index < Methods.Count; ++index)
                Methods[index].MethodId = $"m{index}";
            for (var index = 0; index < Fields.Count; ++index)
                Fields[index].FieldId = $"f{index}";
            for (var index = 0; index < Events.Count; ++index)
                Events[index].EventId = $"e{index}";
        }

        private BfType CreateBfType(TypeDefinition typeDefinition, BfAssembly bfAssembly)
        {
            var key = GetKey(typeDefinition);
            BfType createBfType;

            if (!bfAssembly.GetTypesDictionary().ContainsKey(key))
            {
                createBfType = new BfType(this, typeDefinition, bfAssembly);
                bfAssembly.GetTypesDictionary().Add(GetKey(typeDefinition), createBfType);
                Types.Add(createBfType);
                bfAssembly.Namespaces.Add(createBfType.Namespace);
            }
            else
            {
                createBfType = bfAssembly.GetTypesDictionary()[key];
            }

            return createBfType;
        }

        internal BfNamespace GetNamespace(BfType bfType)
        {
            BfNamespace bfNamespace = null;

            var name = ProcessNamespace(bfType.GetTypeDefinition().FullName);

            if (!_sortedDictionary.ContainsKey(name))
            {
                bfNamespace = new BfNamespace(name);
                _sortedDictionary.Add(name, bfNamespace);
                Namespaces.Add(bfNamespace);
            }
            else
            {
                bfNamespace = _sortedDictionary[name];
            }

            bfNamespace.Types.Add(bfType);

            return bfNamespace;
        }

        private static string ProcessNamespace(string name)
        {
            return name.Contains('.') ? name.Substring(0, name.LastIndexOf('.')) : "";
        }

        internal int GetEventsCount()
        {
            return _eventsCount++;
        }
    }
}