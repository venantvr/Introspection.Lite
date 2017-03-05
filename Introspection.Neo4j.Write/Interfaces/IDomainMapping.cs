using Newtonsoft.Json.Serialization;

namespace Introspection.Neo4j.Write.Interfaces
{
    public interface IDomainMapping
    {
        void Fluent();

        IDomainMapping WithContractResolver(DefaultContractResolver resolver);
    }
}