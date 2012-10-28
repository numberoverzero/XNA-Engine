using System.Threading;

namespace Engine.Utility
{
    /// <summary>
    ///   Extensions for thread actions
    /// </summary>
    public static class ThreadExtensions
    {
        /// <summary>
        ///   Abort a thread if it doesn't join n milliseconds after an interrupt.
        /// </summary>
        /// <param name="thread"> </param>
        /// <param name="millisecondsTimeout"> </param>
        public static void Kill(this Thread thread, int millisecondsTimeout = 0)
        {
            thread.Interrupt();
            if (thread.Join(millisecondsTimeout)) return;
            try
            {
                thread.Abort();
            }
            catch (ThreadAbortException)
            {
            }
            catch (ThreadInterruptedException)
            {
            }
        }
    }
}