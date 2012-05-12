using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    public struct InputSettings
    {
        public float TriggerThreshold;
        public float ThumbstickThreshold;

        public InputSettings(float triggerThreshold, float thumbstickThreshold)
        {
            TriggerThreshold = triggerThreshold;
            ThumbstickThreshold = thumbstickThreshold;
        }
        public InputSettings(InputSettings settings)
        {
            TriggerThreshold = settings.TriggerThreshold;
            ThumbstickThreshold = settings.ThumbstickThreshold;
        }
    }
}
