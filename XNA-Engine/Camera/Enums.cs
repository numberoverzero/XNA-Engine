using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Camera
{
    /// <summary>
    /// The modes that a camera can be in.
    /// 
    /// LockedPos     :: Locked to a single Vector2, does not move
    /// LockedTarget  :: Locked to a single GameObject, tracks the center of the screen to GameObject.Position
    /// VtTTransition :: Vector -> GameObject Transition.  Moving from a LockedPos to LockedTarget
    /// TtVTransition :: GameObject -> Vector Transition.  Moving from a LockedTarget to LockedPos
    /// TtTTransition :: GameObject -> GameObject Transition.  Moving from one LockedPos to another
    /// VtVTransition :: Vector -> Vector Transition.  Moving from one LockedPos to another
    /// </summary>
    public enum CameraMode { 
        /// <summary>
        /// Locked to a single position
        /// </summary>
        LockedPos, 
        
        /// <summary>
        /// Locked to a single GameObject, following that object's position
        /// </summary>
        LockedTarget, 
        
        /// <summary>
        /// Transitioning from a fixed position to tracking a GameObject
        /// </summary>
        VtTTransition,

        /// <summary>
        /// Transitioning from tracking a GameObject to a fixed position
        /// </summary>
        TtVTransition, 
        
        /// <summary>
        /// Transitioning from a fixed position to a different fixed position
        /// </summary>
        TtTTransition, 
        
        /// <summary>
        /// Transitioning from tracking a GameObject to tracking a different GameObject
        /// </summary>
        VtVTransition };
}
