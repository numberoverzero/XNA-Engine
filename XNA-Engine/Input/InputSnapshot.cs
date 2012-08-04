﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    /// <summary>
    /// A snapshot of the polled input devices so that method signatures don't get too cluttered
    /// </summary>
    public struct InputSnapshot
    {
        /// <summary>
        /// The KeyboardState at the time of snapshotting
        /// </summary>
        public readonly KeyboardState KeyboardState;
        /// <summary>
        /// The GamePadState at the time of snapshotting
        /// </summary>
        public readonly GamePadState GamePadState;
        /// <summary>
        /// The MouseState at the time of snapshotting
        /// </summary>
        public readonly MouseState MouseState;
        /// <summary>
        /// The InputSettings at the time of snapshotting
        /// </summary>
        public readonly InputSettings InputSettings;

        /// <summary>
        /// Snapshot a set of input device states
        /// </summary>
        /// <param name="keyboardState"></param>
        /// <param name="gamePadState"></param>
        /// <param name="mouseState"></param>
        /// <param name="inputSettings"></param>
        public InputSnapshot(KeyboardState keyboardState, GamePadState gamePadState, MouseState mouseState, InputSettings inputSettings)
        {
            this.KeyboardState = keyboardState;
            this.GamePadState = gamePadState;
            this.MouseState = mouseState;
            this.InputSettings = inputSettings;
        }
    }
}