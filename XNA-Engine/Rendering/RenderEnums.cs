
namespace Engine.Rendering
{
    /// <summary>
    /// Specify which stage of rendering is occuring
    /// </summary>
    public enum RenderPass { 
        /// <summary>
        /// Nothing happens in this RenderPass (or, an unknown state)
        /// </summary>
        None, 
        /// <summary>
        /// Background items are being rendered
        /// </summary>
        Background, 
        /// <summary>
        /// Just before main special effects are applied
        /// </summary>
        PreEffect, 
        /// <summary>
        /// Any special effects that aren't fullscreen post happen here
        /// </summary>
        Effects, 
        /// <summary>
        /// Anything that happens after effects are applied
        /// </summary>
        PostEffect, 
        /// <summary>
        /// Just before fullscreen postprocessing happens
        /// </summary>
        PrePostProcessing, 
        /// <summary>
        /// Postprocessing - blur, bloom, etc
        /// </summary>
        Post, 
        /// <summary>
        /// Any UI rendered on top of the game world which shouldn't be touched by
        /// postprocessing.  Note that it's fine for UI to have its own effects, but
        /// they may be applied to the game rendering as well (depending on how your
        /// pipeline is configured)
        /// </summary>
        UI, 
        /// <summary>
        /// Anything that should be rendered while debugging
        /// </summary>
        Debug }

    /// <summary>
    /// Draw layer type for an object
    /// </summary>
    public enum LayerType { 
        /// <summary>
        /// A noop layer, or a layer when one is unknown
        /// </summary>
        None, 
        /// <summary>
        /// Basic rendering layer
        /// </summary>
        Base, 
        /// <summary>
        /// Any rendering done on top of the base
        /// </summary>
        Highlight };
}
