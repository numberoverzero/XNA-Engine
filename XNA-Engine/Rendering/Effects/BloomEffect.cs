#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;

#endregion

namespace Engine.Rendering.Effects
{
    /// <summary>
    /// Class holds all the settings used to tweak the bloom effect.
    /// </summary>
    public class BloomSettings
    {
        #region Fields


        // Name of a preset bloom setting, for display to the user.
        public string Name;


        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        public float BloomThreshold;


        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        public float BlurAmount;


        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene. Range 0 to 1.
        public float BloomIntensity;
        public float BaseIntensity;


        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        public float BloomSaturation;
        public float BaseSaturation;


        #endregion


        /// <summary>
        /// Constructs a new bloom settings descriptor.
        /// </summary>
        public BloomSettings(string name, float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation)
        {
            Name = name;
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
        }


        /// <summary>
        /// Table of preset bloom settings, used by the sample program.
        /// </summary>
        public static BloomSettings[] PresetSettings =
        {
            //                Name           Thresh  Blur Bloom  Base  BloomSat BaseSat
            new BloomSettings("Default",     0.25f,  4,   1.25f, 1,    1,       1),
            new BloomSettings("Mild",  0f,     2,   3f,    1,    2,       1),
        };
    }

    public class BloomEffect : RenderEffect
    {
        #region Fields


        private Effect bloomExtractEffect;
        private Effect bloomCombineEffect;
        private Effect gaussianBlurEffect;

        private RenderTarget2D renderTarget1;
        private RenderTarget2D renderTarget2;
        private RenderTarget2D preEffectTextureCopy;

        public BloomSettings Settings;


        #endregion

        #region Constructors


        public BloomEffect() : this(BloomSettings.PresetSettings[0]) { }

        public BloomEffect(BloomSettings bloomSettings) : this(bloomSettings.Name, bloomSettings.BloomThreshold, bloomSettings.BlurAmount,
            bloomSettings.BloomIntensity, bloomSettings.BaseIntensity, bloomSettings.BloomSaturation, bloomSettings.BaseSaturation) { }

        private BloomEffect(string name, float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity, float bloomSaturation, float baseSaturation)
            :base()
        {
            Settings = new BloomSettings(name, bloomThreshold, blurAmount, bloomIntensity, baseIntensity, bloomSaturation, baseSaturation);
        }


        #endregion

        #region Content Management


        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            bloomExtractEffect = contentManager.Load<Effect>("Effects/Bloom/BloomExtract");
            bloomCombineEffect = contentManager.Load<Effect>("Effects/Bloom/BloomCombine");
            gaussianBlurEffect = contentManager.Load<Effect>("Effects/Bloom/GaussianBlur");

            PresentationParameters pp = graphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            preEffectTextureCopy = new RenderTarget2D(graphicsDevice, width, height, false, format, DepthFormat.None);

            width /= 2;
            height /= 2;

            renderTarget1 = new RenderTarget2D(graphicsDevice, width, height, false, format, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(graphicsDevice, width, height, false, format, DepthFormat.None);

            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        public override void UnloadContent()
        {
            bloomExtractEffect = null;
            bloomCombineEffect = null;
            gaussianBlurEffect = null;

            renderTarget1 = null;
            renderTarget2 = null;

            Settings = null;

            base.UnloadContent();
        }


        #endregion

        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            // Pass 1: draw the scene into rendertarget 1, using a
            // shader that extracts only the brightest parts of the image.
            bloomExtractEffect.Parameters["BloomThreshold"].SetValue(
                Settings.BloomThreshold);

            DrawFullscreenQuad(preEffectTexture, renderTarget1, BlendState.Opaque,
                               bloomExtractEffect);

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);

            DrawFullscreenQuad(renderTarget1, renderTarget2, BlendState.Opaque,
                               gaussianBlurEffect);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);

            DrawFullscreenQuad(renderTarget2, renderTarget1, BlendState.Opaque,
                               gaussianBlurEffect);

            // Pass 3.5: copy original scene into rendertarget 2 so that we can sample it.
            // We can't do this like the original bloom, because postEffectTexture is already
            // on the graphics card, so we have to copy to rendertarget 2 and then load that
            // into textures

            DrawFullscreenQuad(preEffectTexture, preEffectTextureCopy, BlendState.Opaque, null);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            graphicsDevice.SetRenderTarget(postEffectTexture);

            EffectParameterCollection parameters = bloomCombineEffect.Parameters;
            parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

            graphicsDevice.Textures[1] = preEffectTextureCopy;

            Viewport viewport = graphicsDevice.Viewport;

            DrawFullscreenQuad(renderTarget1, BlendState.Opaque,
                               viewport.Width, viewport.Height,
                               bloomCombineEffect);
        }

        #region Helper Methods


        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        private float ComputeGaussian(float n)
        {
            float theta = Settings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }


        #endregion

    }
}
