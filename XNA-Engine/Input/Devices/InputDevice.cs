using Microsoft.Xna.Framework;

namespace Engine.Input.Devices
{
    /// <summary>
    ///   Wrapper around different devices, loads info into a snapshot.
    /// </summary>
    public interface InputDevice
    {
        /// <summary>
        ///   Update the device
        /// </summary>
        void Update();

        /// <summary>
        ///   Returns a (likely partial) snapshot of the device(s) for a given player
        /// </summary>
        /// <returns> </returns>
        InputSnapshot GetDeviceSnapshot(PlayerIndex player, FrameState frameState);
    }
}