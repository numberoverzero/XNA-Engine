using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;

namespace Engine.Rendering.Effects
{
    /// <summary>
    /// Repeats a given RenderEffect n times in a row
    /// </summary>
    public class RepeaterEffect : RenderEffect
    {
        #region Fields

        /// <summary>
        /// The effect to repeat
        /// </summary>
        public RenderEffect Effect;
        /// <summary>
        /// The number of times the effect is repeated
        /// </summary>
        public int Repetitions;

        #endregion

        #region Initialization

        /// <summary>
        /// Repeats a noop effect 0 times
        /// </summary>
        public RepeaterEffect() : this(null, 0) { }
        
        /// <summary>
        /// Repeats the given effect once
        /// </summary>
        /// <param name="effect"></param>
        public RepeaterEffect(RenderEffect effect) : this(effect, 1) { }
        
        /// <summary>
        /// Repeats the given effect n times
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="repetitions"></param>
        public RepeaterEffect(RenderEffect effect, int repetitions)
        {
            if (effect == null)
                effect = RenderEffect.None;
            this.Effect = effect;
            Repetitions = repetitions;
        }

        #endregion

        #region Content Management

        /// <summary>
        /// Load any required content the effect requires for rendering
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Effect.LoadContent(contentManager, graphicsDevice, spriteBatch);
            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        /// <summary>
        /// Free up resources when you don't need to render bloom effects anymore.
        /// </summary>
        public override void UnloadContent()
        {
            Effect.UnloadContent();
            base.UnloadContent();
        }

        #endregion

        /// <summary>
        /// Renders the results of the effect on the preEffectTexture to the postEffectTexture.
        /// (preEffectTexture unmodified unless pre == post)
        /// </summary>
        /// <param name="preEffectTexture"></param>
        /// <param name="postEffectTexture"></param>
        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            if (Repetitions > 0)
                for (int i = 0; i < Repetitions; i++ )
                    Effect.ApplyEffect(preEffectTexture, postEffectTexture);
            base.ApplyEffect(preEffectTexture, postEffectTexture);
        }

        /// <summary>
        /// Reset any state the effect might track, such as camera deltas or elapsed game time
        /// </summary>
        public override void Reset()
        {
            Effect.Reset();
            base.Reset();
        }

    }
}
