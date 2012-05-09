#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Engine.Rendering.Pipeline
{
    public class RenderPipeline
    {
        #region Fields

        private static RenderPass[] passes = {RenderPass.Background, RenderPass.PreEffect, RenderPass.Effects, RenderPass.PostEffect, RenderPass.UI};
        /// <summary>
        /// Array of render passes for iteration
        /// </summary>
        public static RenderPass[] Passes
        {
            get
            {
                return passes;
            }
        }

        private Dictionary<RenderPass, RenderTarget2D> passTargetMap;
        private RenderPass renderPass;
        /// <summary>
        /// Which RenderPass the RenderPipeline is currently processing.
        /// Changing this value will trigger at least one render target switch.
        /// </summary>
        public RenderPass RenderPass
        {
            get { return renderPass; }
            set
            {
                renderPass = value;
                RenderTarget = passTargetMap[renderPass];
                graphicsDevice.SetRenderTarget(RenderTarget);
            }
        }

        /// <summary>
        /// The current RenderTarget2D based on which pass the PostPipeline is in
        /// </summary>
        private RenderTarget2D RenderTarget = null;
                
        private List<IRenderEffect> effects;
        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch batch;
        private Game game;

        #endregion

        #region Constructor

        public RenderPipeline(GraphicsDevice graphicsDevice, Game game)
        {
            contentManager = new ContentManager(game.Services, "Content");
            effects = new List<IRenderEffect>();
            
            this.graphicsDevice = graphicsDevice;
            this.game = game;

            passTargetMap = new Dictionary<RenderPass, RenderTarget2D>();

            foreach (var pass in passes)
            {
                RenderTarget2D target;
                GenerateRenderTarget2D(out target);
                passTargetMap.Add(pass, target);
            }
        }
        private void GenerateRenderTarget2D(out RenderTarget2D target)
        {
            PresentationParameters pp = graphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            target = new RenderTarget2D(graphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.PreserveContents);

        }

        #endregion

        #region Content Management

        /// <summary>
        /// Load the pipeline's content
        /// </summary>
        public void LoadContent()
        {
            batch = new SpriteBatch(graphicsDevice);
            foreach (var effect in effects)
                effect.LoadContent(contentManager, graphicsDevice, batch);

        }

        /// <summary>
        /// Unload the pipeline's content, 
        /// and the content of any effects in the pipeline
        /// </summary>
        public void UnloadContent()
        {
            contentManager = null;
            graphicsDevice = null;
            foreach (var pass in passes)
                passTargetMap[pass] = null;
            game = null;

            foreach (var effect in effects)
                effect.UnloadContent();
            effects.Clear();
            effects = null;
            
        }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Clear the screens, set background color
        /// </summary>
        public void Clear(Color color)
        {
            foreach (var pass in passes)
            {
                RenderPass = pass;
                graphicsDevice.Clear(Color.Transparent);
            }
            RenderPass = RenderPass.Background;
            graphicsDevice.Clear(color);
        }

        /// <summary>
        /// Draw everything to the screen
        /// </summary>
        public void Draw()
        {
            ApplyEffects();

            graphicsDevice.SetRenderTarget(null);
            Rectangle destRect = graphicsDevice.Viewport.Bounds;

            batch.Begin(0, BlendState.AlphaBlend);
            foreach (var pass in passes)
            {
                batch.Draw(passTargetMap[pass], destRect, Color.White);
            }
            batch.End();
        }
        private void ApplyEffects()
        {
            RenderTarget2D target = passTargetMap[RenderPass.Effects];
            foreach (var effect in effects)
            {
                effect.ApplyEffect(target, target);
            }
        }

        #endregion

        /// <summary>
        /// Append a rendering effect to the pipeline
        /// and load the effect's content
        /// </summary>
        /// <param name="effect">The effect to be added</param>
        public void AddRenderEffect(IRenderEffect effect)
        {
            effects.Add(effect);
            if (batch == null)
            {
                LoadContent(); 
            }
            else
            { 
                effect.LoadContent(contentManager, graphicsDevice, batch); 
            }
        }

        /// <summary>
        /// Remove a rendering effect from the pipeline
        /// Unloads that effect's content
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveRenderEffect(IRenderEffect effect)
        {
            effects.Remove(effect);
            effect.UnloadContent();
        }
    }
}
