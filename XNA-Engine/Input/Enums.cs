using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    /// <summary>
    /// Used to specify the left or right mouse button
    /// </summary>
    public enum MouseButton { 
        None, 
        Left, 
        Right, 
        Middle }

    /// <summary>
    /// Used to specify the left or right trigger
    /// </summary>
    public enum Trigger { 
        None, 
        Left, 
        Right }

    /// <summary>
    /// Used to specify the left or right thumbstick
    /// </summary>
    public enum Thumbstick { 
        None, 
        Left, 
        Right }

    /// <summary>
    /// Used to specify the direction of a thumbstick "press" (=1)
    /// </summary>
    public enum ThumbstickDirection { 
        None, 
        Up, 
        Down, 
        Left, 
        Right }

    /// <summary>
    /// Which frame (previous or current) you are querying.
    /// Used when asking about a key's state
    /// </summary>
    public enum FrameState { 
        /// <summary>
        /// The frame which occured before whatever is considered the "current"
        /// </summary>
        Previous, 
        /// <summary>
        /// The frame that logic is being run in right now (or graphics are being rendered in, depending on context)
        /// </summary>
        Current }
}
