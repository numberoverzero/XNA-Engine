using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;
using Engine.Utility;

namespace Engine.DataStructures
{
    public class ByteArrayBuilder
    {
        private readonly bool _autoAppendTermCharToStrings;
        private readonly List<byte[]> _pieces = new List<byte[]>();
        private readonly char _termChar;

        public ByteArrayBuilder(bool autoAppendTermCharToStrings = true, char termChar = '\0')
        {
            _autoAppendTermCharToStrings = autoAppendTermCharToStrings;
            _termChar = termChar;
        }

        public void Add(byte[] bytes)
        {
            _pieces.Add(bytes);
        }

        public void Add(bool b)
        {
            Add(b.AsByteArray());
        }

        public void Add(int i)
        {
            Add(i.AsByteArray());
        }

        public void Add(string s, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            if (_autoAppendTermCharToStrings) s = s.WithTermChar(_termChar);
            Add(s.AsByteArray(encoding));
        }

        public void Add(IByteSerializeable byteSerializeable)
        {
            Add(byteSerializeable.AsByteArray());
        }

        public void Add<T>(List<T> list) where T : IByteSerializeable
        {
            foreach (var t in list)
                Add(t);
        }

        public byte[] GetByteArray()
        {
            var bufSize = _pieces.Sum(segment => segment.Length);
            var bytes = new byte[bufSize];

            var destinationIndex = 0;
            foreach (var piece in _pieces)
            {
                var length = piece.Length;
                Array.Copy(piece, 0, bytes, destinationIndex, length);
                destinationIndex += length;
            }
            return bytes;
        }
    }
}