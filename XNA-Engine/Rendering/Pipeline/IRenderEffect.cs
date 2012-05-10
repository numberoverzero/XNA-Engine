#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Engine.Rendering.Pipeline
{
    public interface IRenderEffect
    {
        void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
        void UnloadContent(); 
        
        void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture);
    }
}
