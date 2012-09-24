using System;

namespace Engine.Utility
{
    /// <summary>
    ///   A function which is closed over floats
    ///   (they don't necessarily make guarantees about accuracy)
    /// </summary>
    public interface FloatFn
    {
        /// <summary>
        ///   Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t"> The point at which the function is being evaluated </param>
        /// <returns> </returns>
        float At(float t);
    }

    /// <summary>
    ///   Returns values which oscillate on initial + [-mag, mag]
    /// </summary>
    public class OscillatingFn : FloatFn
    {
        private readonly float freq;
        private readonly float initial;
        private readonly float mag;

        /// <summary>
        ///   Returns values which oscillate on initial + [-mag, mag]
        /// </summary>
        public OscillatingFn(float initial, float mag, float freq)
        {
            this.initial = initial;
            this.mag = mag;
            this.freq = freq;
        }

        #region FloatFn Members

        /// <summary>
        ///   Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t"> The point at which the function is being evaluated </param>
        /// <returns> </returns>
        public virtual float At(float t)
        {
            float offset = mag*(float) Math.Sin(t*freq*2*Math.PI);
            return initial + offset;
        }

        #endregion
    }

    /// <summary>
    ///   Returns values of the form initial + mag*e^(-d*t)
    /// </summary>
    public class DecayFn : FloatFn
    {
        private readonly float decayCoeff;
        private readonly float initial;
        private readonly float mag;

        /// <summary>
        ///   Returns values of the form initial + mag*e^(-d*t)
        /// </summary>
        public DecayFn(float initial, float mag, float decayCoeff)
        {
            this.initial = initial;
            this.mag = mag;
            this.decayCoeff = decayCoeff;
        }

        #region FloatFn Members

        /// <summary>
        ///   Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t"> The point at which the function is being evaluated </param>
        /// <returns> </returns>
        public virtual float At(float t)
        {
            float offset = mag*(float) Math.Exp(-decayCoeff*t);
            return initial + offset;
        }

        #endregion
    }

    public static class MathExtensions
    {
        public static int Mod(this int a, float b)
        {
            return (int) (a - b*Math.Floor(a/b));
        }

        public static float Mod(this float a, float b)
        {
            return (float) (a - b*Math.Floor(a/b));
        }

        public static int WrappedIndex(int index, int size)
        {
            return index.Mod(size);
        }
    }
}