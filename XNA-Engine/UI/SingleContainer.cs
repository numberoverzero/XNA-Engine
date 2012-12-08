using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.UI
{
    public class SingleContainer : Container
    {
        public SingleContainer(string name) : base(name)
        {
        }

        public SingleContainer(string name, float x, float y, float w, float h, bool expandable, Alignment? align,
                               IEnumerable<Widget> children) : base(name, x, y, w, h, expandable, align, children)
        {
        }

        public Widget Content
        {
            get { return NumberOfChildren > 0 ? Children[0] : null; }
        }

        public override void AddChild(Widget child)
        {
            RemoveAllChildren();
            base.AddChild(child);
        }

        public override void DetermineSize()
        {
            if (Content == null) return;
            Content.DetermineSize();
            PreferredSize = Content.PreferredSize;
        }

        public override void ResetSize(Vector2 size)
        {
            base.ResetSize(size);
            if (Content != null) Content.ResetSize(size);
        }
    }
}