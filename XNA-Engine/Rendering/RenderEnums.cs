
namespace Engine.Rendering
{
    /// <summary>
    /// Specify which stage of rendering is occuring
    /// </summary>
    public enum RenderPass { None, Background, PreEffect, Effects, PostEffect, PrePostProcessing, Post, UI, Debug }

    /// <summary>
    /// Draw layer type for an object
    /// </summary>
    public enum LayerType { None, Base, Highlight };
}
