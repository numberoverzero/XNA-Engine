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

        bool IsActive(string bindingName, FrameState state = FrameState.Current);
        bool IsPressed(string bindingName);
        bool IsReleased(string bindingName);

        IEnumerable<IBinding> GetModifiers { get; }

        // Multiple Binding Query

        bool AnyActive(params string[] keys);
        bool AllActive(params string[] keys);
        
        bool AnyPressed(params string[] keys);
        bool AllPressed(params string[] keys);
        
        bool AnyReleased(params string[] keys);
        bool AllReleased(params string[] keys);

        // Per-frame input polling

        void Update();
    }
}
