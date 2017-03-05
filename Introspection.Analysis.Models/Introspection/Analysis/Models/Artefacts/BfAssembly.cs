using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

// ReSharper disable InconsistentNaming

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [Serializable]
    public class BfAssembly : IDisposable
    {
        [NonSerialized] private AssemblyDefinition _assemblyDefinition;

        private string _assemblyId;

        private Dictionary<string, BfMethod> _methodsDictionary = new Dictionary<string, BfMethod>();
        private ISymbolReader _symbolReader;
        private Dictionary<string, BfType> _typesDictionary = new Dictionary<string, BfType>();

        internal BfAssembly(AssemblyDefinition assemblyDef, bool isCoreAssembly, string rootDirectory)
        {
            _assemblyDefinition = assemblyDef;

            Name = assemblyDef.Name.Name;
            IsCoreAssembly = isCoreAssembly;
            Version = _assemblyDefinition.Name.Version.ToString();

            try
            {
                var dllFileName = Path.Combine(rootDirectory, $"{_assemblyDefinition.Name.Name}.dll");
                var exeFileName = Path.Combine(rootDirectory, $"{_assemblyDefinition.Name.Name}.exe");

                string file = null;

                if (File.Exists(dllFileName))
                    file = dllFileName;
                else if (File.Exists(exeFileName))
                    file = exeFileName;

                _symbolReader = new PdbReaderProvider().GetSymbolReader(null, file);
            }
                // ReSharper disable once EmptyGeneralCatchClause
                // ReSharper disable once UnusedVariable
            catch (Exception ex)
            {
            }
        }

        internal BfAssembly()
        {
        }

        public string AssemblyId
        {
            // ReSharper disable once UnusedMember.Global
            get { return _assemblyId; }
            internal set { _assemblyId = value; }
        }

        public string Version { get; }

        public NamespaceCollection Namespaces { get; } = new NamespaceCollection();

        public string Name { get; }

        public bool IsCoreAssembly { get; }

        public void Dispose()
        {
            if (_symbolReader != null)
            {
                _symbolReader.Dispose();
                _symbolReader = null;
            }

            _assemblyDefinition = null;
            _typesDictionary = null;
            _methodsDictionary = null;
        }

        public override string ToString()
        {
            return $"IAssembly: {Name}";
        }

        [SpecialName]
        internal AssemblyDefinition GetAssemblyDefinition()
        {
            return _assemblyDefinition;
        }

        [SpecialName]
        internal Dictionary<string, BfType> GetTypesDictionary()
        {
            return _typesDictionary;
        }

        [SpecialName]
        internal Dictionary<string, BfMethod> GetMethodsDictionary()
        {
            return _methodsDictionary;
        }
    }
}