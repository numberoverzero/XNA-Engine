using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    /// <summary>
    ///   A snapshot of the polled input devices so that method signatures don't get too cluttered
    /// </summary>
    public class InputSnapshot
    {
        /// <summary>
        ///   The GamePadState at the time of snapshotting
        /// </summary>
        public readonly GamePadState? GamePadState;

        /// <summary>
        ///   The InputSettings at the time of snapshotting
        /// </summary>
        public readonly InputSettings InputSettings;

        /// <summary>
        ///   The KeyboardState at the time of snapshotting
        /// </summary>
        public readonly KeyboardState? KeyboardState;

        /// <summary>
        ///   The MouseState at the time of snapshotting
        /// </summary>
        public readonly MouseState? MouseState;

        /// <summary>
        ///   Snapshot a set of input device states
        /// </summary>
        public InputSnapshot(KeyboardState? keyboardState, GamePadState? gamePadState, MouseState? mouseState,
                             InputSettings inputSettings)
        {
            KeyboardState = keyboardState;
            GamePadState = gamePadState;
            MouseState = mouseState;
            InputSettings = inputSettings;
        }

        /// <summary>
        ///   Creates a new snapshot where non-null values of the other snapshot overwrite the values from this snapshot.
        /// </summary>
        public InputSnapshot Merge(InputSnapshot other)
        {
            return Merge(this, other);
        }

        /// <summary>
        ///   Merge the values of two snapshots into a new snapshot, where non-null values from the second overwrite those of the first.
        /// </summary>
        public static InputSnapshot Merge(InputSnapshot snapshot1, InputSnapshot snapshot2)
        {
            var keyboardState = snapshot2.KeyboardState ?? snapshot1.KeyboardState;
            var gamePadState = snapshot2.GamePadState ?? snapshot1.GamePadState;
            var mouseState = snapshot2.MouseState ?? snapshot1.MouseState;
            var inputSettings = snapshot2.InputSettings ?? snapshot1.InputSettings;
            return new InputSnapshot(keyboardState, gamePadState, mouseState, inputSettings);
        }

        /// <summary>
        ///   Snapshot a set of input device states
        /// </summary>
        public static InputSnapshot With(KeyboardState keyboardState)
        {
            return new InputSnapshot(keyboardState, null, null, null);
        }

        /// <summary>
        ///   Snapshot a set of input device states
        /// </summary>
        public static InputSnapshot With(GamePadState gamePadState)
        {
            return new InputSnapshot(null, gamePadState, null, null);
        }

        /// <summary>
        ///   Snapshot a set of input device states
        /// </summary>
        public static InputSnapshot With(MouseState mouseState)
        {
            return new InputSnapshot(null, null, mouseState, null);
        }

        /// <summary>
        ///   Snapshot a set of input device states
        /// </summary>
        public static InputSnapshot With(InputSettings inputSettings)
        {
            return new InputSnapshot(null, null, null, inputSettings);
        }
    }
}