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
        Vector2 GetMousePosition(FrameState state = FrameState.Current);

        // Binding Mutation

        void AddBinding(string bindingName, IBinding binding, PlayerIndex player = PlayerIndex.One);

        void RemoveBinding(string bindingName, int index, PlayerIndex player = PlayerIndex.One);
        void RemoveBinding(string bindingName, IBinding binding, PlayerIndex player = PlayerIndex.One);

        bool ContainsBinding(string bindingName, PlayerIndex player = PlayerIndex.One);
        
        void ClearBinding(string bindingName, PlayerIndex player = PlayerIndex.One);
        void ClearAllBindings();

        // Single Binding Query

        bool IsActive(string bindingName, PlayerIndex player = PlayerIndex.One, FrameState state = FrameState.Current);
        bool IsPressed(string bindingName, PlayerIndex player = PlayerIndex.One);
        bool IsReleased(string bindingName, PlayerIndex player = PlayerIndex.One);
        List<IBinding> GetCurrentBindings(string bindingName, PlayerIndex player = PlayerIndex.One);

        // Manager Query
        List<string> BindingsUsing(IBinding binding, PlayerIndex player = PlayerIndex.One);

        // Per-frame input polling

        void Update();

        // Modifiers

        IEnumerable<IBinding> GetModifiers { get; }
    }
}
