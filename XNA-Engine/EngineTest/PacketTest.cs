using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Engine.Networking.Packets;

namespace EngineTest
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void TestNewPacket()
        {
            ChatPacket expected = new ChatPacket();
            expected.From = "Tester";
            expected.To = "Testee";
            expected.Message = @"This is a test. ! a /\&..,";
            byte[] buffer = expected.ByteStream;

            Packet actual = Packet.Parse(buffer);
            Assert.IsInstanceOfType(actual, typeof(ChatPacket));
            ChatPacket cActual = actual as ChatPacket;
            Assert.AreEqual<ChatPacket>(expected, cActual);
        }
    }
}
