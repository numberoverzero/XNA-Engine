using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Utility;

namespace Engine.Camera
{
    #region CameraEffects

    /// <summary>
    /// Modifies the position, scale, or rotation of a camera.
    /// Care should be taken that a CameraEffect never lets net scale = 0
    /// </summary>
    public class CameraEffect
    {
        protected Camera camera;
        protected float elapsed;
        protected float dur;

        /// <summary>
        /// Percent of effect elapsed- t in [0, 1]
        /// </summary>
        protected float t
        {
            get
            {
                if (dur == 0)
                    return 0;
                return MathHelper.Clamp(elapsed / dur, 0, 1);
            }
        }

        public bool IsActive
        {
            get { return dur > 0 && elapsed <= dur; }
        }

        public CameraEffect(Camera camera, float dur)
        {
            elapsed = 0;
            this.camera = camera;
            this.dur = dur / 1000f;
        }

        public virtual void Update(float dt)
        {
            elapsed += dt;
        }

        public virtual void Reset()
        {
            elapsed = 0;
        }

        public virtual void End()
        {
            elapsed = dur = 0;
            camera = null;
        }

        public float[] Offsets
        {
            get
            {
                float[] offsets = new float[5];
                offsets[0] = offsetX();
                offsets[1] = offsetY();
                offsets[2] = rotation();
                offsets[3] = scaleX();
                offsets[4] = scaleY();
                return offsets;
            }
        }

        protected virtual float offsetX() { return 0; }
        protected virtual float offsetY() { return 0; }

        /// <summary>
        /// Always multiply by camera scale so that we don't hit a scale of 0.
        /// Original equation was scale *= (1 + offsetScale)
        /// New equation is scale += offsetScale
        /// We can keep the functionality by ultiplying by camera scale in this equation
        /// </summary>
        /// <returns></returns>
        protected virtual float scaleX() { return 0; }

        /// <summary>
        /// Always multiply by camera scale so that we don't hit a scale of 0.
        /// Original equation was scale *= (1 + offsetScale)
        /// New equation is scale += offsetScale
        /// We can keep the functionality by ultiplying by camera scale in this equation
        /// </summary>
        /// <returns></returns>
        protected virtual float scaleY() { return 0; }

        protected virtual float rotation() { return 0; }
    }

    /// <summary>
    /// This effect doesn't impact the camera in any way
    /// However, it keeps the CameraEffect cleaner (not all effects need freq, decay, mag)
    /// and prevents repitition of variables in subclasses (Shake, Bounce)
    /// </summary>
    public class DampedOscillationEffect : CameraEffect
    {
        protected float decay;
        protected float mag;
        protected float freq;

        public DampedOscillationEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur)
        {
            this.decay = decay;
            this.mag = mag;
            this.freq = freq;
        }
    }

    /// <summary>
    /// Creates a radial shake effect, using damped oscillation
    /// One pass goes from 0 -> mag -> 0 -> -mag -> 0
    /// Specify the number of passes to make via frequency
    /// </summary>
    public class CameraShakeEffect : DampedOscillationEffect
    {
        protected FloatFn magFunction;
        protected FloatFn oscillateFunction;

        /// <summary>
        /// Applys a rotation effect that oscillates from -mag to +mag and decays over time.
        /// </summary>
        /// <param name="camera">The camera the effect is applied to</param>
        /// <param name="dur">Duration in ms for the effect to occur over</param>
        /// <param name="decay">Decay coeffecient of the shaking.  Use 0 for no decay</param>
        /// <param name="mag">Maximum magnitude in radians for the camera to rotate</param>
        /// <param name="freq">Number of times the camera makes a full left -> right -> left pass</param>
        public CameraShakeEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur, decay, mag, freq)
        {
            magFunction = new DecayFn(0, 1, decay);
            oscillateFunction = new OscillatingFn(0, 1, freq);
        }

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
        protected FloatFn magFunction;
        protected FloatFn oscillateFunction;

        /// <summary>
        /// Applys a scale effect that oscillates from (1 - mag) to (1 + mag) and decays over time.
        /// </summary>
        /// <param name="camera">The camera the effect is applied to</param>
        /// <param name="dur">Duration in ms for the effect to occur over</param>
        /// <param name="decay">Decay coeffecient of the zoom.  Use 0 for no decay</param>
        /// <param name="mag">Maximum magnitude in percent original zoom to move by</param>
        /// <param name="freq">Number of times the camera makes a full in -> out -> in pass</param>
        public CameraBounceEffect(Camera camera, float dur, float decay, float mag, float freq)
            : base(camera, dur, decay, mag, freq)
        {
            magFunction = new DecayFn(0, 1, decay);
            oscillateFunction = new OscillatingFn(0, 1, freq);
        }

        protected override float scaleX()
        {
            return camera.Scale.X * scale();
        }
        protected override float scaleY()
        {
            return camera.Scale.Y * scale();
        }
        private float scale()
        {
            return mag * magFunction.At(t) * oscillateFunction.At(t);
        }

    }

    /// <summary>
    /// Flips the camera about its x- or y-axis, or both.
    /// Because this effect modifies camera scale directly, 
    /// take care when combining with other effects that modify scale.
    /// </summary>
    public class CameraFlipEffect : CameraEffect
    {
        protected Vector2 tempScale;
        protected bool flipX, flipY;
        protected float adjust;

        public CameraFlipEffect(Camera camera, float dur, bool flipX, bool flipY)
            : base(camera, dur)
        {
            adjust = -1f;

            this.flipX = flipX;
            this.flipY = flipY;

            SetScale(adjust);
        }

        public override void End()
        {
            SetScale(1 / adjust);
            base.End();
        }

        private void SetScale(float scale)
        {
            tempScale = camera.Scale;
            if (flipX)
                tempScale.X *= scale;
            if (flipY)
                tempScale.Y *= scale;
            camera.Scale = tempScale;
        }
    }

    #endregion
}
