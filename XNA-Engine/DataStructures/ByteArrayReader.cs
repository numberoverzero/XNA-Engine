using System;
using System.Collections.Generic;
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

        /// <summary>
        ///   Tries to read an IByteSerializeable of the given type, using that type's FromByteArray
        /// </summary>
        /// <typeparam name="T"> The type to try to read </typeparam>
        /// <param name="t"> Where the value is read to. Should not be used if the method returns false </param>
        /// <returns> True if the value was successfully read, false otherwise </returns>
        public bool TryRead<T>(out T t) where T : IByteSerializeable, new()
        {
            t = new T();
            var endIndex = t.FromByteArray(_bytes, Index);
            if (endIndex < Index) return false;
            Index = endIndex;
            return true;
        }

        /// <summary>
        ///   Tries to read a list of values.
        /// </summary>
        public bool TryReadList<T>(out List<T> list) where T : IByteSerializeable, new()
        {
            list = new List<T>();
            // Number of elements
            var n = ReadInt32();
            for (; n > 0; n--)
            {
                T t;
                var success = TryRead(out t);
                if (!success)
                {
                    list.Clear();
                    return false;
                }
                list.Add(t);
            }
            return true;
        }

        /// <summary>
        ///   Perform an unsafe read, where success is assumed
        /// </summary>
        public T Read<T>() where T : IByteSerializeable, new()
        {
            T t;
            TryRead(out t);
            return t;
        }

        /// <summary>
        ///   Perform an unsafe read, where success is assumed
        /// </summary>
        public List<T> ReadList<T>() where T : IByteSerializeable, new()
        {
            List<T> list;
            TryReadList(out list);
            return list;
        }

        public List<int> ReadIntList()
        {
            var list = new List<int>();
            // Number of elements
            var n = ReadInt32();
            for(;n>0;n--)
                list.Add(ReadInt32());
            return list;
        }

        public List<string> ReadStringList(char terminatingChar = '\0')
        {
            var list = new List<string>();
            // Number of elements
            var n = ReadInt32();
            for (; n > 0; n--)
                list.Add(ReadString(terminatingChar));
            return list;
        }

        public void Reset()
        {
            Index = 0;
        }
    }
}