using Engine.Networking.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void TestNewPacket()
        {
            var expected = new ChatPacket {From = "Tester", To = "Testee", Message = @"This is a test. ! a /\&..,"};
            byte[] buffer = expected.ByteStream;

            Packet actual = Packet.Parse(buffer);
            Assert.IsInstanceOfType(actual, typeof (ChatPacket));
            var cActual = actual as ChatPacket;
            Assert.AreEqual(expected, cActual);
        }
    }
}