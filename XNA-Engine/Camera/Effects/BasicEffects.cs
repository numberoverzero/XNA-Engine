using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Utility;

namespace Engine.Camera.Effects
{
    /// <summary>
    /// This effect doesn't impact the camera in any way
    /// However, it keeps the CameraEffect cleaner (not all effects need freq, decay, mag)
    /// and prevents repitition of variables in subclasses (Shake, Bounce)
    /// </summary>
    public abstract class DampedOscillationEffect : CameraEffect
    {
        #region Fields

        /// <summary>
        /// Decay coeffecient in expoenential decay function of magnitude
        /// </summary>
        protected float decay;
        /// <summary>
        /// Magnitude
        /// </summary>
        protected float mag;
        /// <summary>
        /// Oscillation frequency
        /// </summary>
        protected float freq;

        /// <summary>
        /// A function that modifies the magnitude of the bounce over time
        /// </summary>
        protected FloatFn magFunction;
        /// <summary>
        /// A function that describes the oscillation of the bounce over time
        /// </summary>
        protected FloatFn oscillateFunction;

        #endregion

        /// <summary>
        /// An effect which oscillates between [-mag, +mag] and whose mag decays over time
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="dur"></param>
        /// <param name="decay"></param>
        /// <param name="mag"></param>
        /// <param name="freq"></param>
        public DampedOscillationEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur)
        {
            this.decay = decay;
            this.mag = mag;
            this.freq = freq;
            magFunction = new DecayFn(0, 1, decay);
            oscillateFunction = new OscillatingFn(0, 1, freq);
        }
    }

    /// <summary>
    /// Creates a radial shake effect, using damped oscillation
    /// One pass goes from 0 -> mag -> 0 -> -mag -> 0
    /// Specify the number of passes to make via frequency
    /// </summary>
    public class CameraShakeEffect : DampedOscillationEffect
    {
        /// <summary>
        /// Applys a rotation effect that oscillates from -mag to +mag and decays over time.
        /// </summary>
        /// <param name="camera">The camera the effect is applied to</param>
        /// <param name="dur">Duration in ms for the effect to occur over</param>
        /// <param name="decay">Decay coeffecient of the shaking.  Use 0 for no decay</param>
        /// <param name="mag">Maximum magnitude in radians for the camera to rotate</param>
        /// <param name="freq">Number of times the camera makes a full left -> right -> left pass</param>
        public CameraShakeEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur, decay, mag, freq) { }

        /// <summary>
        /// Camera's rotation offset
        /// </summary>
        /// <returns></returns>
        protected override float rotation()
        {
            return mag * magFunction.At(t) * oscillateFunction.At(t);
        }

    }

    /// <summary>
    /// Creates a 'pop' or bounce effect, using damped oscillation
    /// One pass goes from 0 -> cameraScale * mag -> 0 -> -cameraScale * mag -> 0
    /// Specify the number of passes to make via frequency
    /// </summary>
    public class CameraBounceEffect : DampedOscillationEffect
    {
        /// <summary>
        /// Applys a scale effect that oscillates from (1 - mag) to (1 + mag) and decays over time.
        /// </summary>
        /// <param name="camera">The camera the effect is applied to</param>
        /// <param name="dur">Duration in ms for the effect to occur over</param>
        /// <param name="decay">Decay coeffecient of the zoom.  Use 0 for no decay</param>
        /// <param name="mag">Maximum magnitude in percent original zoom to move by</param>
        /// <param name="freq">Number of times the camera makes a full in -> out -> in pass</param>
        public CameraBounceEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur, decay, mag, freq) { }

        /// <summary>
        /// Camera's horizontal scale offset
        /// </summary>
        /// <returns></returns>
        protected override float scaleX()
        {
            return camera.Scale.X * scale();
        }

        /// <summary>
        /// Camera's vertical scale offset
        /// </summary>
        /// <returns></returns>
        protected override float scaleY()
        {
            return camera.Scale.Y * scale();
        }

        private float scale()
        {
            return mag * magFunction.At(t) * oscillateFunction.At(t);
        }
    }
}
