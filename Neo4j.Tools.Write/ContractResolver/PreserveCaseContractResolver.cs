using Newtonsoft.Json.Serialization;

namespace Neo4j.Tools.Write.ContractResolver
{
    public class PreserveCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName;
        }
    }
}