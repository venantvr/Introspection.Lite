using System.Runtime.Serialization;

namespace Introspection.Dumper.Dto
{
    public class SimpleType
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        [IgnoreDataMember]
        public SimpleNamespace Namespace { get; set; }

        public string BusinessId { get; set; }
    }
}