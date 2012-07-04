#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Engine.Input
{
    public interface IInputManager
    {
        Vector2 GetMousePosition(FrameState state = FrameState.Current);

        // Binding Mutation

        void AddBinding(string bindingName, IBinding binding);
        void RemoveBinding(string bindingName);
        bool ContainsBinding(string bindingName);
        void ClearAllBindings();

        // Single Binding Query

        bool IsActive(string bindingName, PlayerIndex player = PlayerIndex.One, FrameState state = FrameState.Current);
        bool IsPressed(string bindingName, PlayerIndex player = PlayerIndex.One);
        bool IsReleased(string bindingName, PlayerIndex player = PlayerIndex.One);

        // Multiple Binding Query

        bool AnyActive(PlayerIndex player = PlayerIndex.One, params string[] keys);
        bool AllActive(PlayerIndex player = PlayerIndex.One, params string[] keys);
        
        bool AnyPressed(PlayerIndex player = PlayerIndex.One, params string[] keys);
        bool AllPressed(PlayerIndex player = PlayerIndex.One, params string[] keys);
        
        bool AnyReleased(PlayerIndex player = PlayerIndex.One, params string[] keys);
        bool AllReleased(PlayerIndex player = PlayerIndex.One, params string[] keys);

        // Per-frame input polling

        void Update();

        // Modifiers

        IEnumerable<IBinding> GetModifiers { get; }
    }
}
