using System;
using System.Text;
using Engine.Serialization;
using Engine.Utility;

namespace Engine.DataStructures
{
    public class ByteArrayReader
    {
        private readonly byte[] _bytes;

        public ByteArrayReader(byte[] bytes, int startIndex)
        {
            _bytes = bytes;
            Index = startIndex;
        }

        public int Index { get; private set; }

        public bool ReadBool()
        {
            var b = BitConverter.ToBoolean(_bytes, Index);
            Index++;
            return b;
        }

        public int ReadInt32()
        {
            var b = BitConverter.ToInt32(_bytes, Index);
            Index += 4;
            return b;
        }

        /// <summary>
        ///   String must be UTF8-Encoded
        /// </summary>
        /// <returns> </returns>
        public string ReadString(char terminatingChar = '\0')
        {
            var terminatingIndex = _bytes.IndexOf(Index, terminatingChar);
            if (terminatingIndex < 0)
                throw new IndexOutOfRangeException("Did not find terminating char before end of string");
            var length = terminatingIndex - Index;
            var str = Encoding.UTF8.GetString(_bytes, Index, length);

            // We return the string w/o the terminating char, but we still advance the cursor past that char.
            Index += length + 1;
            return str;
        }

        public bool TryRead<T>(out T t) where T : IByteSerializeable, new()
        {
            t = new T();
            var endIndex = t.FromByteArray(_bytes, Index);
            if (endIndex < Index) return false;
            Index = endIndex + 1;
            return true;
        }

        /// <summary>
        ///   Perform an unsafe read, where success is assumed
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public T Read<T>() where T : IByteSerializeable, new()
        {
            T t;
            TryRead(out t);
            return t;
        }

        public void Reset()
        {
            Index = 0;
        }
    }
}