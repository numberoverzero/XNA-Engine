using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Engine.Utility;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    public class SpriteSheet
    {
        public ContentManager Content;
        public string MappingFilename;
        public bool Loaded { get; protected set; }
        protected Dictionary<string, Sprite> SpriteMapping; 

        /// <summary>
        /// <para>Loads the texture from a given file.  Will use parameters if passed, and class values if not.</para>
        /// <para>If both content and this.Content are null, doesn't do anything</para>
        /// <para>mappingFile should be an XML file generated with TexturePacker</para>
        /// <para>Assumes non-rotated images, chops .png from image names when accessing</para>
        /// </summary>
        public void LoadContent(ContentManager content = null, string mappingFilename = null)
        {
            SpriteMapping = new Dictionary<string, Sprite>();
            Loaded = false;
            Content = content ?? Content;
            MappingFilename = mappingFilename ?? MappingFilename;

            if (Content == null || String.IsNullOrEmpty(MappingFilename)) return;

            var doc = new XmlDocument();
            doc.Load(MappingFilename);
            var root = doc["TextureAtlas"];
            if (root == null) return;
            var imagePath = root.GetAttribute("imagePath");
            var texture = Content.Load<Texture2D>(imagePath.Until(".png"));
            foreach (XmlElement spriteNode in root.GetElementsByTagName("sprite"))
            {
                var name = spriteNode.GetAttribute("n").Until(".png");
                var x = spriteNode.GetAttribute("x").ToInt();
                var y = spriteNode.GetAttribute("y").ToInt();
                var w = spriteNode.GetAttribute("w").ToInt();
                var h = spriteNode.GetAttribute("h").ToInt();
                var sprite = new Sprite {Name = name, X = x, Y = y, W = w, H = h, Texture = texture};
                SpriteMapping[name] = sprite;
            }
            Loaded = true;
        }

        public bool Contains(string spriteName)
        {
            return SpriteMapping.ContainsKey(spriteName);
        }

        public Sprite this[string spriteName]
        {
            get { return Loaded && Contains(spriteName) ? SpriteMapping[spriteName] : Sprite.NullSprite; }
        }
    }
}
