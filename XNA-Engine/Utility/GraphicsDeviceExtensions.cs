#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Engine.Utility
{
    public static class GraphicsDeviceExtensions
    {
        /// <summary>
        /// Because this method takes a boolean, it will never pass a RenderTargetUsage of PlatformContents,
        /// only PreserveContents or DiscardContents.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="preserveContents"></param>
        /// <returns></returns>
        public static RenderTarget2D CreateFullscreenRenderTarget(this GraphicsDevice graphicsDevice, bool preserveContents)
        {
            var renderTargetUsage = preserveContents ? RenderTargetUsage.PreserveContents : RenderTargetUsage.DiscardContents;
            return CreateFullscreenRenderTarget(graphicsDevice, renderTargetUsage);
        }

        public static RenderTarget2D CreateFullscreenRenderTarget(this GraphicsDevice graphicsDevice, RenderTargetUsage renderTargetUsage)
        {
            PresentationParameters pp = graphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            return new RenderTarget2D(graphicsDevice, width, height, false, format, 
                                      pp.DepthStencilFormat, pp.MultiSampleCount, 
                                      renderTargetUsage);
            
        }

    }
}
