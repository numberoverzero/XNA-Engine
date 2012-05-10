using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;
using Engine.Utility;

namespace Engine.Rendering.Effects
{
    public class BlurEffect : RenderEffect
    {
        #region Fields

        private RenderTarget2D combineTarget; 
        private RenderTarget2D previousFrame;
        private Effect reduceAlphaEffect;

        private Vector2 renderDimensions;
        public float AlphaDecay = 0.003f;

        #endregion

        #region Constructors


        public BlurEffect() : this(0) { }
        public BlurEffect(float alphaDecay) : base()
        {
            AlphaDecay = alphaDecay;
        }


        #endregion

        #region Content Management


        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            reduceAlphaEffect = contentManager.Load<Effect>("Effects/Blur/ReduceAlpha");

            renderDimensions = new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth,
                                           graphicsDevice.PresentationParameters.BackBufferHeight);
            
            previousFrame = graphicsDevice.CreateFullscreenRenderTarget(true);
            combineTarget = graphicsDevice.CreateFullscreenRenderTarget(false);

            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        public override void UnloadContent()
        {
            reduceAlphaEffect = null;
            previousFrame = null;
            combineTarget = null;

            base.UnloadContent();
        }


        #endregion

        /// <summary>
        /// This should most likely be called every frame, to update the change in position of the camera.
        /// This will make the blur render w.r.t. the player.  Of course, you could also blur by a normal to
        /// the camera delta or an oscillating angle to the player, to create different effects.
        /// </summary>
        /// <param name="delta">Change in position of the blur effect since the last frame.  
        ///                     Do not scale the offset to texture coordinates, that will be done for you.</param>
        public void SetFrameDelta(Vector2 delta)
        {
            reduceAlphaEffect.Parameters["offsetXY"].SetValue(delta / renderDimensions);
        }

        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            reduceAlphaEffect.Parameters["factor"].SetValue(AlphaDecay);

            // Pass 1: render the lastFrame onto combineTarget using the reduceAlphaEffect to "fade out" the last draw data.
            
            graphicsDevice.Textures[0] = previousFrame;
            DrawFullscreenQuad(previousFrame, combineTarget, BlendState.Opaque, reduceAlphaEffect);

            // Pass 2: render the contents of this latest frame (preEffectTexture) to the renderTarget1
            DrawFullscreenQuad(preEffectTexture, combineTarget, BlendState.AlphaBlend, null);

            // Pass 3: copy everything from the combineTarget to the lastFrame for the next draw
            DrawFullscreenQuad(combineTarget, previousFrame, BlendState.Opaque, null);

            // Pass 4: copy the combineTarget to the preEffectTexture
            DrawFullscreenQuad(combineTarget, postEffectTexture, BlendState.NonPremultiplied, null);
        }
    }
}
