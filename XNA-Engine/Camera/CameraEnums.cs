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
    public enum CameraMode { LockedPos, LockedTarget, VtTTransition, TtVTransition, TtTTransition, VtVTransition };
}
