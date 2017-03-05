using System;
using System.Diagnostics;
using System.Linq;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Collections
{
    [DebuggerDisplay("TypeCollection: {Count}")]
    [Serializable]
    public class TypeCollection : BaseCollection<BfType>
    {
        internal override void Clear()
        {
            base.Clear();

            _data.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
        }

        public override string ToString()
        {
            return $"TypeCollection: {this.Count()} items";
        }
    }
}