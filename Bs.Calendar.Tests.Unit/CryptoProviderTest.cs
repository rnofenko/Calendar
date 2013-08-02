using Bs.Calendar.Rules;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class CryptoProviderTest
    {
        private ICryptoProvider _cryptoProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _cryptoProvider = new KeccakCryptoProvider();
        }

        [TestCase("",
            Result =
                "0eab42de4c3ceb9235fc91acffe746b29c29a8c366b7c60e4e67c466f36a4304c00fa9caf9d87976ba469bcbe06713b435f091ef2769fb160cdab33d3670680e"
            ),
         TestCase("The quick brown fox jumps over the lazy dog",
             Result =
                 "d135bb84d0439dbac432247ee573a23ea7d3c9deb2a968eb31d47c4fb45f1ef4422d6c531b5b9bd6f449ebcc449ea94d0a8f05f62130fda612da53c79659f609"
             )]
        public string ShouldReturnCorrectKeccakHash(string data)
        {
            // act & assert
            return _cryptoProvider.GetHash(data).ToLower();
        }
    }
}
