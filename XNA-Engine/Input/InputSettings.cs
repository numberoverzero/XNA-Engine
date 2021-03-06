﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    /// <summary>
    /// A set of limits and thresholds that some bindings must pass to be considered active
    /// </summary>
    public class InputSettings
    {
        /// <summary>
        /// The minimum value for a trigger to register as 'pulled'
        /// </summary>
        public float TriggerThreshold;
        /// <summary>
        /// The minimum value for a thumbstick (not necessarily magnitude) to register as 'pushed'
        /// </summary>
        public float ThumbstickThreshold;

        /// <summary>
        /// Construct a set of Input Settings describing certain thresholds and limits that must be met for
        /// some bindings to be considered "active"
        /// </summary>
        /// <param name="triggerThreshold">The minimum value a trigger must have to be considered pulled/depressed (active)</param>
        /// <param name="thumbstickThreshold">The minimum value a thumbstick must be moved to be considered active 
        /// (some bindings may require this amount in a single direction)</param>
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
