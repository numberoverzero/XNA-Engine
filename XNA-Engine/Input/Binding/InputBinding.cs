#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#endregion

namespace Engine.Input
{
    /// <summary>
    /// See <see cref="IBinding"/>
    /// </summary>
    public abstract class InputBinding : IBinding
    {
        /// <summary>
        /// Any modifiers required for this binding to be considered 'active'
        /// </summary>
        public IBinding[] Modifiers { get; private set; }

        #region Initialiation

        /// <summary>
        /// Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
        /// 
        public InputBinding(params IBinding[] modifiers)
        {
            Modifiers = new IBinding[modifiers.Length];
            Array.Copy(modifiers, Modifiers, modifiers.Length);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public InputBinding(InputBinding other) : this(other.Modifiers) { }

        #endregion

        /// <summary>
        /// See <see cref="IBinding.IsActive"/>
        /// </summary>
        public abstract bool IsActive(InputSnapshot inputSnapshot);
    }
}
