using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI
{
    /// <summary>
    /// Nested dictionary for referring to elements on a spritesheet by name
    /// </summary>
    public class Theme
    {
        public ContentManager Content;
        protected Texture2D Texture;
        public string AssetFilename;
        public string MappingFilename;
        public bool Loaded { get; protected set; }

        /// <summary>
        /// <para>Loads the texture from a given file.  Will use parameters if passed, and class values if not.</para>
        /// <para>If both content and this.Content are null, doesn't do anything</para>
        /// </summary>
        public void LoadContent(ContentManager content = null, string assetFilename = null, string mappingFilename = null)
        {
            Loaded = false;
            content = content ?? Content;
            assetFilename = assetFilename ?? AssetFilename;
            mappingFilename = mappingFilename ?? MappingFilename;

            if (content == null || String.IsNullOrEmpty(assetFilename) || String.IsNullOrEmpty(mappingFilename)) return;
            
            //Load image
            Texture = content.Load<Texture2D>(assetFilename);

            //Load mapping
            LoadMapping(mappingFilename);

            Loaded = true;
        }

        protected void LoadMapping(string mappingFilename)
        {
            throw new NotImplementedException();
        }
    }
}
