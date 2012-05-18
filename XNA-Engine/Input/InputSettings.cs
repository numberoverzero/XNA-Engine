using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    public struct InputSettings
    {
        /// <summary>
        /// The minimum value for a trigger to register as 'pulled'
        /// </summary>
        public float TriggerThreshold;
        /// <summary>
        /// The minimum value for a thumbstick (not necessarily magnitude) to register as 'pushed'
        /// </summary>
        public float ThumbstickThreshold;

        public InputSettings(float triggerThreshold, float thumbstickThreshold)
        {
            TriggerThreshold = triggerThreshold;
            ThumbstickThreshold = thumbstickThreshold;
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="settings"></param>
        public InputSettings(InputSettings settings)
        {
            TriggerThreshold = settings.TriggerThreshold;
            ThumbstickThreshold = settings.ThumbstickThreshold;
        }
    }
}
