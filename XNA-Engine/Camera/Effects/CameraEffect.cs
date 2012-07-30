using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Utility;

namespace Engine.Camera.Effects
{
    /// <summary>
    /// Modifies the position, scale, or rotation of a camera.
    /// Care should be taken that a CameraEffect never lets net scale = 0
    /// </summary>
    public class CameraEffect
    {
        #region Fields

        /// <summary>
        /// The Camera the effect is being applied to
        /// </summary>
        protected Camera camera;
        /// <summary>
        /// Elapsed time in seconds of the effect
        /// </summary>
        protected float elapsed;
        /// <summary>
        /// Duration in seconds of the effect
        /// </summary>
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

        /// <summary>
        /// True if 0 &lt; dur &lt;= elapsed
        /// </summary>
        public bool IsActive
        {
            get { return dur > 0 && elapsed <= dur; }
        }

        #endregion

        /// <summary>
        /// Construct a camera effect on the target camera for dur seconds
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="dur"></param>
        public CameraEffect(Camera camera, float dur)
        {
            elapsed = 0;
            this.camera = camera;
            this.dur = dur / 1000f;
        }

        /// <summary>
        /// Update the camera effect - elapsed time may be incremented past duration
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {
            elapsed += dt;
        }

        /// <summary>
        /// Reset the camera effect to its original state
        /// </summary>
        public virtual void Reset()
        {
            elapsed = 0;
        }

        /// <summary>
        /// End the camera effect immediately.
        /// </summary>
        public virtual void End()
        {
            elapsed = 0;
            camera = null;
        }

        #region Offsets

        /// <summary>
        /// Returns the offsets used in calculating the camera transform matrix
        /// </summary>
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

        /// <summary>
        /// x pos offset of the camera caused by the effect
        /// </summary>
        /// <returns></returns>
        protected virtual float offsetX() { return 0; }

        /// <summary>
        /// y pos offset of the camera caused by the effect
        /// </summary>
        /// <returns></returns>
        protected virtual float offsetY() { return 0; }

        /// <summary>
        /// Always multiply by camera scale so that we don't hit a scale of 0.
        /// Original equation was scale *= (1 + offsetScale)
        /// New equation is scale += offsetScale
        /// We can keep the functionality by multiplying by camera scale in this equation
        /// </summary>
        /// <returns></returns>
        protected virtual float scaleX() { return 0; }

        /// <summary>
        /// Always multiply by camera scale so that we don't hit a scale of 0.
        /// Original equation was scale *= (1 + offsetScale)
        /// New equation is scale += offsetScale
        /// We can keep the functionality by multiplying by camera scale in this equation
        /// </summary>
        /// <returns></returns>
        protected virtual float scaleY() { return 0; }

        /// <summary>
        /// Rotation of the camera caused by the effect
        /// </summary>
        /// <returns></returns>
        protected virtual float rotation() { return 0; }

        #endregion
    }
}
