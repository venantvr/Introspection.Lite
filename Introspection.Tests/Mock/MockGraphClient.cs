using System;
using Neo4jClient;
using Neo4jClient.Execution;

namespace Introspection.Tests.Mock
{
    public class MockGraphClient : GraphClient
    {
        public MockGraphClient(Uri rootUri, IHttpClient httpClient) : base(rootUri, httpClient)
        {
        }

        public MockGraphClient(Uri rootUri, string username = null, string password = null) : base(rootUri, username, password)
        {
        }

        public MockGraphClient(Uri rootUri, bool expect100Continue, bool useNagleAlgorithm, string username = null, string password = null) : base(rootUri, expect100Continue, useNagleAlgorithm, username, password)
        {
        }

        public new bool IsConnected => true;

        public new void Connect(NeoServerConfiguration configuration = null)
        {
        }
    }
}