using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.DataStructures
{
    /// <summary>
    /// A function which is closed over floats
    /// (they don't necessarily make guarantees about accuracy)
    /// </summary>
    public interface FloatFn
    {
        /// <summary>
        /// Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t">The point at which the function is being evaluated</param>
        /// <returns></returns>
        float At(float t);
    }

    /// <summary>
    /// Returns values which oscillate on initial + [-mag, mag]
    /// </summary>
    public class OscillatingFn : FloatFn
    {
        float initial;
        float mag;
        float freq;

        /// <summary>
        /// Returns values which oscillate on initial + [-mag, mag]
        /// </summary>
        public OscillatingFn(float initial, float mag, float freq)
        {
            this.initial = initial;
            this.mag = mag;
            this.freq = freq;
        }

        /// <summary>
        /// Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t">The point at which the function is being evaluated</param>
        /// <returns></returns>
        public virtual float At(float t)
        {
            float offset = mag * (float)Math.Sin(t * freq * 2 * Math.PI);
            return initial + offset;
        }
    }

    /// <summary>
    /// Returns values of the form initial + mag*e^(-d*t)
    /// </summary>
    public class DecayFn : FloatFn
    {
        float initial;
        float mag;
        float decayCoeff;

        /// <summary>
        /// Returns values of the form initial + mag*e^(-d*t)
        /// </summary>
        public DecayFn(float initial, float mag, float decayCoeff)
        {
            this.initial = initial;
            this.mag = mag;
            this.decayCoeff = decayCoeff;
        }

        /// <summary>
        /// Returns the value of the function at a given t value
        /// </summary>
        /// <param name="t">The point at which the function is being evaluated</param>
        /// <returns></returns>
        public virtual float At(float t)
        {
            float offset = mag * (float)Math.Exp(-decayCoeff * t);
            return initial + offset;
        }

    }
}
