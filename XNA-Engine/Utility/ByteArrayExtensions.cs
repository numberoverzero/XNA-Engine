using System;
using System.Text;

namespace Engine.Utility
{
    public static class ByteArrayExtensions
    {
        public static byte[] WithSizePrefix(this byte[] buffer)
        {
            var length = buffer.Length;
            var sizeBuffer = new byte[length + 4];
            Array.Copy(length.AsByteArray(), sizeBuffer, 4);
            Array.Copy(buffer, 0, sizeBuffer, 4, length);
            return sizeBuffer;
        }

        public static byte[] AsByteArray(this bool b)
        {
            return BitConverter.GetBytes(b);
        }

        public static byte[] AsByteArray(this int i)
        {
            return BitConverter.GetBytes(i);
        }

        public static byte[] AsByteArray(this char c, Encoding encoding)
        {
            return encoding.GetBytes(new[] {c});
        }

        public static byte[] AsByteArray(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static byte[] AsByteArray(this string str)
        {
            return str.AsByteArray(Encoding.UTF8);
        }

        public static int IndexOf(this byte[] buffer, int startingIndex, char c)
        {
            var b = c.AsByteArray(Encoding.UTF8);
            for (int i = startingIndex; i < buffer.Length; i++)
            {
                if (buffer[i] != b[0]) continue;
                var match = true;
                for (int j = 0; j < b.Length; j++)
                {
                    // This was our last chance at a match and we didn't find one.
                    if (i + j >= buffer.Length) return -1;

                    // Initially our sequence matched, but it doesn't anymore
                    if (buffer[i + j] != b[j]) match = false;

                    // Exit early
                    if (!match) break;
                }
                if (match) return i;
            }
            return -1;
        }
    }
}