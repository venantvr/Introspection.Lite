using System.Diagnostics;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models
{
    [DebuggerDisplay("AssemblyTuple: {Assembly.Name.Name}")]
    public class AssemblyTuple
    {
        private AssemblyDefinition _assemblyDefinition;

        public AssemblyTuple(AssemblyDefinition assembly)
        {
            Assembly = assembly;
        }

        public AssemblyDefinition Assembly
        {
            get { return _assemblyDefinition; }
            private set
            {
                if (_assemblyDefinition == value)
                    return;
                _assemblyDefinition = value;
                IsCoreAssembly = File
                    .ReadAllLines(@"Exclusions.txt")
                    .All(e => !Assembly.Name.Name.StartsWith(e));
            }
        }

        public bool IsCoreAssembly { get; set; }

        public string Directory { get; set; }
    }
}