using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Neo4j.Tools.Write.Interfaces;

namespace Neo4j.Tools.Write.Hash
{
    public class BuiltInTypeHashProcessor : IHashProcessor
    {
        public string GetHash<T>(T instance)
        {
            var cryptoServiceProvider = new MD5CryptoServiceProvider();

            var builtInProperties = typeof (T).GetProperties()
                .Where(p => p.PropertyType.Namespace == "System")
                .Select(p => new CustomKeyValuePair<string, object>(p.Name, p.GetValue(instance, null)))
                .ToArray();

            var serializer = new DataContractSerializer(builtInProperties.GetType());

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, builtInProperties);
                cryptoServiceProvider.ComputeHash(memoryStream.ToArray());
                return Convert.ToBase64String(cryptoServiceProvider.Hash);
            }
        }

        [Serializable]
        private struct CustomKeyValuePair<TK, TV>
        {
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public TK Key { get; set; }
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public TV Value { get; set; }

            public CustomKeyValuePair(TK key, TV value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}