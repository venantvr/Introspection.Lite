using Introspection.Dumper.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4j.Tools.Write.Hash;

namespace Introspection.Tests
{
    [TestClass]
    public class HashTest
    {
        [TestMethod]
        public void BuiltInTypeHash_01()
        {
            var method_01 = new SimpleMethod
                            {
                                BusinessId = "hello",
                                FullName = @"world"
                            };

            var method_02 = new SimpleMethod
                            {
                                BusinessId = "hello",
                                FullName = @"world"
                            };

            var hasher = new BuiltInTypeHashProcessor();

            Assert.AreEqual(hasher.GetHash(method_01), hasher.GetHash(method_02));
        }

        [TestMethod]
        public void BuiltInTypeHash_02()
        {
            var method_01 = new SimpleMethod
                            {
                                BusinessId = "hello",
                                FullName = @"world"
                            };

            var method_02 = new SimpleMethod
                            {
                                BusinessId = "hello",
                                FullName = @"freaks"
                            };

            var hasher = new BuiltInTypeHashProcessor();

            Assert.AreNotEqual(hasher.GetHash(method_01), hasher.GetHash(method_02));
        }
    }
}