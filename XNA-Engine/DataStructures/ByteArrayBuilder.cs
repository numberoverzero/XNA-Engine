using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Serialization;

namespace Engine.DataStructures
{
    public class ByteArrayBuilder
    {
        private readonly List<byte[]> _pieces = new List<byte[]>();

        public void Add(byte[] bytes)
        {
            _pieces.Add(bytes);
        }

        public void Add(IByteSerializeable byteSerializeable)
        {
            Add(byteSerializeable.AsByteArray());
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