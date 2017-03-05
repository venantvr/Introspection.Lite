using System;
using System.Diagnostics;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [DebuggerDisplay("BfNamespace: {Name}")]
    [Serializable]
    public class BfNamespace
    {
        private string _namespaceId;
        private TypeCollection _typeCollection = new TypeCollection();

        internal BfNamespace(string fullname)
        {
            Name = fullname;
        }

        internal BfNamespace()
        {
        }

        public string Name { get; }

        public TypeCollection Types
        {
            get { return _typeCollection; }
            // ReSharper disable once UnusedMember.Global
            internal set { _typeCollection = value; }
        }

        public string NamespaceId
        {
            get { return _namespaceId; }
            internal set { _namespaceId = value; }
        }

        public override string ToString()
        {
            return $"INamespace: {Name}";
        }
    }
}