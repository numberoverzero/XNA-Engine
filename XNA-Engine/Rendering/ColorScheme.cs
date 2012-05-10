#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace Engine.Rendering
{
    public class ColorScheme<T>
    {
        /// <summary>
        /// Stores all of the [T, LayerType] => Color information
        /// </summary>
        Dictionary<T, Dictionary<LayerType, Color>> colorMapping;

        /// <summary>
        /// Create a new OrbColorScheme
        /// </summary>
        public ColorScheme()
        {
            colorMapping = new Dictionary<T, Dictionary<LayerType, Color>>();
        }

        /// <summary>
        /// Gets or sets the Color associated with the key and layer
        /// </summary>
        /// <param name="hostility"></param>
        /// <param name="layer"></param>
        /// <returns>Color defined for specific key/layer</returns>
        public Color this[T t, LayerType layer]
        {
            get{
                if (colorMapping.ContainsKey(t) && colorMapping[t].ContainsKey(layer))
                    return colorMapping[t][layer];
                return default(Color);
            }
            set{
                if (!colorMapping.ContainsKey(t))
                    colorMapping[t] = new Dictionary<LayerType, Color>();
                colorMapping[t][layer] = value;
            }
        }   

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
