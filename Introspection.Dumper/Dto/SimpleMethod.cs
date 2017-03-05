using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Introspection.Dumper.Dto
{
    public class SimpleMethod
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public SimpleType Type { get; set; }
        public string BusinessId { get; set; }

        [IgnoreDataMember]
        public IEnumerable<SimpleMethod> Calls { get; set; }
    }
}