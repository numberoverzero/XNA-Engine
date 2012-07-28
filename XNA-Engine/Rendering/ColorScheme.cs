#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Utility;

#endregion

namespace Engine.Rendering
{
    /// <summary>
    /// Maps LayerTypes and <typeparamref name="T"/>s to Colors.
    /// </summary>
    /// <typeparam name="T">The type which maps to Colors for a certain LayerType</typeparam>
    public class ColorScheme<T> : MultiKeyDict<T, LayerType, Color>
    {
        /// <summary>
        /// Create an empty ColorScheme mapping
        /// </summary>
        public ColorScheme() : base() { }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public ColorScheme(ColorScheme<T> other) : base(other) { }

        /// <summary>
        /// Load a color scheme from a file
        /// </summary>
        /// <param name="filename">Location of the color scheme file</param>
        /// <param name="profilename">The profile to load from the color scheme file</param>
        public void LoadFromFile(string filename, string profilename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save a color scheme to a file
        /// </summary>
        /// <param name="filename">Location to save the color scheme file</param>
        /// <param name="profilename">The profile to save the color scheme as, in the file</param>
        public void SaveToFile(string filename, string profilename)
        {
            throw new NotImplementedException();
        }
    }
}
