using System;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [Serializable]
    public abstract class BfMember
    {
        // ReSharper disable once InconsistentNaming
        protected readonly BfCache _cache;
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once InconsistentNaming
        protected readonly BfType _type;
        // ReSharper disable once InconsistentNaming
        protected readonly TypeCollection _typesUsed = new TypeCollection();

        internal BfMember()
        {
        }

        internal BfMember(BfCache cache, MemberReference memberRef, BfType type)
        {
            Name = memberRef.Name;
            FullName = $"{type.FullName}.{Name}";
            _cache = cache;
            _type = type;
        }

        public BfType Type => _type;

        public string Name { get; }

        public string FullName { get; }

        public TypeCollection TypesUsed => _typesUsed;
    }
}