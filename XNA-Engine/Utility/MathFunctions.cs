using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    public interface MathFunction
    {
        float At(float t);
    }

    public class OscillatingFunction : MathFunction
    {
        float initial;
        float mag;
        float freq;

        public OscillatingFunction(float initial, float mag, float freq)
        {
            this.initial = initial;
            this.mag = mag;
            this.freq = freq;
        }

        public virtual float At(float t)
        {
            float offset = mag * (float)Math.Sin(t * freq * 2 * Math.PI);
            return initial + offset;
        }
    }

    public class DecayFunction : MathFunction
    {
        float initial;
        float mag;
        float decayCoeff;

        public DecayFunction(float initial, float mag, float decayCoeff)
        {
            this.initial = initial;
            this.mag = mag;
            this.decayCoeff = decayCoeff;
        }

        public virtual float At(float t)
        {
            float offset = mag * (float)Math.Exp(-decayCoeff * t);
            return initial + offset;
        }

    }
}
