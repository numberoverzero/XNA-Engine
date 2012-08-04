using System;
using System.ComponentModel;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions related to Invocation
    /// </summary>
    public static class InvokeExtensions
    {
        /// <summary>
        /// If performing the action would require a synchronized invoke,
        /// does that for you.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="action"></param>
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }

}
