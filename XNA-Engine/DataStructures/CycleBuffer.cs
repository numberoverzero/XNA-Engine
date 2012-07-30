using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.DataStructures
{
    /// <summary>
    /// A cyclic buffer - cycling will move the values in Current into Previous,
    /// and clear the Current buffer.
    /// Both buffers can be written to and inspected at any time.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CycleBuffer<TKey, TValue>
    {
        TKey keyCurrent, keyPrevious;
        /// <summary>
        /// Previous values (will be cleared on next Cycle)
        /// </summary>
        public ISet<TValue> Previous { get; protected set; }
        /// <summary>
        /// Current values (will be moved to Previous on next Cycle)
        /// </summary>
        public ISet<TValue> Current { get; protected set; }

        /// <summary>
        /// Construct a CycleBuffer with the given current/previous keys
        /// </summary>
        /// <param name="keyCurrent"></param>
        /// <param name="keyPrevious"></param>
        public CycleBuffer(TKey keyCurrent, TKey keyPrevious)
        {
            this.keyCurrent = keyCurrent;
            this.keyPrevious = keyPrevious;
            Previous = new HashSet<TValue>();
            Current = new HashSet<TValue>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cycleBuffer"></param>
        public CycleBuffer(CycleBuffer<TKey, TValue> cycleBuffer)
        {
            keyCurrent = cycleBuffer.keyCurrent;
            keyPrevious = cycleBuffer.keyPrevious;
            Previous = new HashSet<TValue>(cycleBuffer.Previous);
            Current = new HashSet<TValue>(cycleBuffer.Current);
        }

        /// <summary>
        /// Returns the requested buffer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ISet<TValue> this[TKey key]
        {
            get
            {
                if (key.Equals(keyCurrent))
                    return Current;
                else if (key.Equals(keyPrevious))
                    return Previous;
                else
                    return null;
            }
        }

        /// <summary>
        /// Add a value to one of the buffers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            var buffer = this[key];
            if (buffer == null) return;
            buffer.Add(value);
        }

        /// <summary>
        /// Clear both buffers
        /// </summary>
        public void Clear()
        {
            Current.Clear();
            Previous.Clear();
        }

        /// <summary>
        /// Move current values to previous and clear current
        /// </summary>
        public void Cycle()
        {
            var temp = Previous;
            Previous = Current;
            Current = temp;
            Current.Clear();
        }

        /// <summary>
        /// Remove a value from one of the buffers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Remove(TKey key, TValue value)
        {
            var buffer = this[key];
            if (buffer == null) return;
            buffer.Remove(value);
        }
    }

    /// <summary>
    /// A cyclic buffer - cycling will move the values in Current into Previous,
    /// and clear the Current buffer.
    /// Both buffers can be written to and inspected at any time.
    /// </summary>
    /// <typeparam name="TBufferKey"></typeparam>
    /// <typeparam name="TBufferInnerKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CycleBuffer<TBufferKey, TBufferInnerKey, TValue>
    {
        TBufferKey keyCurrent, keyPrevious;
        Func<List<TValue>> defaultValueFunc = () => { return new List<TValue>(); };

        /// <summary>
        /// Previous values (will be cleared on next Cycle)
        /// </summary>
        public DefaultDict<TBufferInnerKey, List<TValue>> Previous { get; protected set; }
        /// <summary>
        /// Current values (will be moved to Previous on next Cycle)
        /// </summary>
        public DefaultDict<TBufferInnerKey, List<TValue>> Current { get; protected set; }

        /// <summary>
        /// Construct a CycleBuffer with the given current/previous keys
        /// </summary>
        /// <param name="keyCurrent"></param>
        /// <param name="keyPrevious"></param>
        public CycleBuffer(TBufferKey keyCurrent, TBufferKey keyPrevious)
        {
            this.keyCurrent = keyCurrent;
            this.keyPrevious = keyPrevious;
            Previous = new DefaultDict<TBufferInnerKey, List<TValue>>(defaultValueFunc);
            Current = new DefaultDict<TBufferInnerKey, List<TValue>>(defaultValueFunc);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="cycleBuffer"></param>
        public CycleBuffer(CycleBuffer<TBufferKey, TBufferInnerKey, TValue> cycleBuffer)
        {
            keyCurrent = cycleBuffer.keyCurrent;
            keyPrevious = cycleBuffer.keyPrevious;
            Previous = new DefaultDict<TBufferInnerKey, List<TValue>>(cycleBuffer.Previous);
            Current = new DefaultDict<TBufferInnerKey, List<TValue>>(cycleBuffer.Current);
        }

        /// <summary>
        /// Returns the requested buffer
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public IList<TValue> this[TBufferKey key1, TBufferInnerKey key2]
        {
            get
            {
                if (key1.Equals(keyCurrent))
                    return Current[key2];
                else if (key1.Equals(keyPrevious))
                    return Previous[key2];
                else
                    return null;
            }
        }

        /// <summary>
        /// Add a value to one of the buffers
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        public void Add(TBufferKey key1, TBufferInnerKey key2, TValue value)
        {
            var buffer = this[key1, key2];
            if (buffer == null) return;
            buffer.Add(value);
        }

        /// <summary>
        /// Clear both buffers
        /// </summary>
        public void Clear()
        {
            Current.Clear();
            Previous.Clear();
        }

        /// <summary>
        /// Move current values to previous and clear current
        /// </summary>
        public void Cycle()
        {
            var temp = Previous;
            Previous = Current;
            Current = temp;
            Current.Clear();
        }

        /// <summary>
        /// Remove a value from one of the buffers
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        public void Remove(TBufferKey key1, TBufferInnerKey key2, TValue value)
        {
            var buffer = this[key1, key2];
            if (buffer == null) return;
            buffer.Remove(value);
        }
    }
}
