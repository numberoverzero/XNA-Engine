using System;
using System.Collections.Generic;

namespace Engine.Mathematics
{
    public static class Basics
    {
        public static long GCD(long a, long b)
        {
            while (b != 0)
            {
                var t = b;
                b = a%b;
                a = t;
            }
            return a;
        }

        public static long LCM(long a, long b)
        {
            return (a*b)/GCD(a, b);
        }

        public static int Mod(int a, float b)
        {
            return (int) (a - b*Math.Floor(a/b));
        }

        public static float Mod(float a, float b)
        {
            return (float) (a - b*Math.Floor(a/b));
        }

        public static int WrappedIndex(int index, int size)
        {
            return Mod(index, size);
        }
    }
}