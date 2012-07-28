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
        /// <summary>
        /// No mouse button
        /// </summary>
        None, 
        /// <summary>
        /// Left mouse button
        /// </summary>
        Left, 
        /// <summary>
        /// Right mouse button
        /// </summary>
        Right, 
        /// <summary>
        /// Middle click (not scroll up/down)
        /// </summary>
        Middle }

    /// <summary>
    /// Used to specify the left or right trigger
    /// </summary>
    public enum Trigger { 
        /// <summary>
        /// No trigger
        /// </summary>
        None, 
        /// <summary>
        /// Left trigger (as viewed from top)
        /// </summary>
        Left, 
        /// <summary>
        /// Right trigger (as viewed from top)
        /// </summary>
        Right }

    /// <summary>
    /// Used to specify the left or right thumbstick
    /// </summary>
    public enum Thumbstick { 
        /// <summary>
        /// No thumbstick
        /// </summary>
        None, 
        /// <summary>
        /// Left thumbstick (as viewed from top)
        /// </summary>
        Left, 
        /// <summary>
        /// Right thumbstick (as viewed from top)
        /// </summary>
        Right }

    /// <summary>
    /// Used to specify the direction of a thumbstick "press" (=1)
    /// </summary>
    public enum ThumbstickDirection { 
        /// <summary>
        /// No direction
        /// </summary>
        None, 
        /// <summary>
        /// Controller up
        /// </summary>
        Up, 
        /// <summary>
        /// Controller down
        /// </summary>
        Down, 
        /// <summary>
        /// Controller left
        /// </summary>
        Left, 
        /// <summary>
        /// Controller right
        /// </summary>
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
