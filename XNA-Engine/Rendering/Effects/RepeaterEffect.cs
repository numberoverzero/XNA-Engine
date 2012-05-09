using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;

namespace Engine.Rendering.Effects
{

    public class RepeaterEffect : RenderEffect
    {
        public RenderEffect effect;
        public int Repetitions;

        public RepeaterEffect() : this(null, 0) { }
        public RepeaterEffect(RenderEffect effect) : this(effect, 1) { }
        public RepeaterEffect(RenderEffect effect, int repetitions)
        {
            this.effect = effect;
            Repetitions = repetitions;
        }

        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (effect != null)
                effect.LoadContent(contentManager, graphicsDevice, spriteBatch);
            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        public override void UnloadContent()
        {
            if (effect != null)
                effect.UnloadContent();
            base.UnloadContent();
        }

        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            if (effect != null && Repetitions > 0)
                for (int i = 0; i < Repetitions; i++ )
                    effect.ApplyEffect(preEffectTexture, postEffectTexture);
            base.ApplyEffect(preEffectTexture, postEffectTexture);
        }

    }
}
