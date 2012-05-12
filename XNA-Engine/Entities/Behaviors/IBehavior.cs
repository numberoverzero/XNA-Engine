using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Entities.Behaviors
{
    public interface IBehavior
    {
        void Apply(GameObject gameObject);
        void Destroy();
        bool Enabled { get; set; }
        bool MeetsCriteria(GameObject gameObject);
        void Update(float dt);
    }
}
