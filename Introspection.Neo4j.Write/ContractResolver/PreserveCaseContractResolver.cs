using Newtonsoft.Json.Serialization;

namespace Introspection.Neo4j.Write.ContractResolver
{
    public class PreserveCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName;
        }
    }
}