using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    /// <summary>
    /// Used to specify the left or right mouse button
    /// </summary>
    public enum MouseButton { None, Left, Right, Middle }

    /// <summary>
    /// Used to specify the left or right trigger
    /// </summary>
    public enum Trigger { None, Left, Right }

    /// <summary>
    /// Used to specify the left or right thumbstick
    /// </summary>
    public enum Thumbstick { None, Left, Right }

    /// <summary>
    /// Used to specify the direction of a thumbstick "press" (=1)
    /// </summary>
    public enum ThumbstickDirection { None, Up, Down, Left, Right }

    /// <summary>
    /// Specifies the type of binding an InputBinding has.
    /// </summary>
    /// <remarks>
    /// This allows high-level checks for the action associated with the Input,
    /// instead of worrying about the details of that input's type
    /// 
    /// Thumbstick is registered on ANY thumbstick activity with magnitude greater than ThumbstickThreshold
    /// ThumbstickDirection is registered when the specified thumbstick is moved past Threshold in that direction
    /// </remarks>
    public enum BindingType { None, Key, Button, Trigger, Thumbstick, ThumbstickDirection, MouseButton }

    /// <summary>
    /// Which frame (previous or current) you are querying.
    /// Used when asking about a key's state
    /// </summary>
    public enum FrameState { Previous, Current }
}
