using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.DataStructures;

namespace Engine.Serialization
{
    public abstract class ByteSerializeable : IByteSerializeable
    {
        public byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            BuildAsByteArray(b);
            return b.GetByteArray();
        }

        public int FromByteArray(byte[] bytes, int startIndex)
        {
            var reader = new ByteArrayReader(bytes, startIndex);
            return ReadFromByteArray(reader);
        }

        public abstract void BuildAsByteArray(ByteArrayBuilder builder);
        protected abstract int ReadFromByteArray(ByteArrayReader reader);
    }
}
