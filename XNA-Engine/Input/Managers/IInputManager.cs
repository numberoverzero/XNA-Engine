#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Engine.Input
{
    // TODO Add comments
    public interface IInputManager
    {
        Vector2 GetMousePosition(FrameState state);

        // Binding Mutation

        void AddBinding(string bindingName, IBinding binding, PlayerIndex player);

        void RemoveBinding(string bindingName, int index, PlayerIndex player);
        void RemoveBinding(string bindingName, IBinding binding, PlayerIndex player);

        bool ContainsBinding(string bindingName, PlayerIndex player);
        
        void ClearBinding(string bindingName, PlayerIndex player);
        void ClearAllBindings();

        // Single Binding Query

        bool IsActive(string bindingName, PlayerIndex player, FrameState state);
        bool IsPressed(string bindingName, PlayerIndex player);
        bool IsReleased(string bindingName, PlayerIndex player);
        List<IBinding> GetCurrentBindings(string bindingName, PlayerIndex player);

        // Manager Query
        List<string> BindingsUsing(IBinding binding, PlayerIndex player);

        // Per-frame input polling

        void Update();

        // Modifiers

        IEnumerable<IBinding> GetModifiers { get; }
    }
}
