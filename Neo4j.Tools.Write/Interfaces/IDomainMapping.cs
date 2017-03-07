using Newtonsoft.Json.Serialization;

namespace Neo4j.Tools.Write.Interfaces
{
    public interface IDomainMapping
    {
        void Fluent();

        IDomainMapping WithContractResolver(DefaultContractResolver resolver);
    }
}