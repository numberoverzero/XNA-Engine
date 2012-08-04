using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.DataStructures
{
    /// <summary>
    /// Provides Push, Flip, and full Front inspection.
    /// Does not support Pop or buffer folding (extend front with back instead of swapping and clearing)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleBuffer<T>
    {
        /// <summary>
        /// The current Front buffer.  Cannot be written to.
        /// </summary>
        public List<T> Front { get; protected set; }

        /// <summary>
        /// The current Back buffer.  Can be written to.
        /// </summary>
        protected List<T> Back { get; set; }

        /// <summary>
        /// Returns the number of items currently in the back buffer
        /// </summary>
        public int BackBufferSize
        {
            get { return Back.Count; }
        }

        /// <summary>
        /// Construct an empty DoubleBuffer
        /// </summary>
        public DoubleBuffer()
        {
            Front = new List<T>();
            Back = new List<T>();
            Back.Clear();
        }

        /// <summary>
        /// Flip Front and Back buffers
        /// </summary>
        public void Flip()
        {
            var temp = Front;
            Front = Back;
            Back = temp;
            Back.Clear();

        }

        /// <summary>
        /// Push a value onto the back buffer
        /// </summary>
        /// <param name="item">The item to push</param>
        public void Push(T item)
        {
            Back.Add(item);
        }

        
    }
}
