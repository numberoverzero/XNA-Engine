using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Tiles
{
    /// <summary>
    /// Tracks buffered chunks and point/chunk updating
    /// </summary>
    public class TileManager<TValue>
    {
        #region Fields

        /// <summary>
        /// Chunks of tiles loaded in memory
        /// </summary>
        Chunk<TValue>[] Chunks { get; set; }

        /// <summary>
        /// The dimensions of chunks buffered.
        /// </summary>
        /// <example>
        /// For BufferDimensions (3,3)
        /// we would buffer the following:
        /// .....
        /// .+++.
        /// .+++.
        /// .+++.
        /// .....
        /// where   "." is unbuffered,
        ///         "+" is buffered
        /// </example>
        public Point BufferDimensions { get; protected set; }

        /// <summary>
        /// The size of a single chunk
        /// </summary>
        public Point ChunkDimensions { get; protected set; }

        #endregion


        #region Initialization

        /// <summary>
        /// Create a TileManager which tracks n chunks, where n is the product of the buffer dimensions.
        /// Each chunk has m tiles, where m is the product of the chunk dimensions
        /// </summary>
        /// <param name="bufferDimensions"></param>
        /// <param name="chunkDimensions"></param>
        public TileManager(Point bufferDimensions, Point chunkDimensions)
        {
            BufferDimensions = bufferDimensions; 
            ChunkDimensions = chunkDimensions;
            InitializeChunks();
        }

        void InitializeChunks()
        {
            int nchunks = BufferDimensions.X * BufferDimensions.Y;
            Chunks = new Chunk<TValue>[nchunks];
            for (int i = 0; i < nchunks; i++)
                Chunks[i] = new Chunk<TValue>(Point.Zero, ChunkDimensions);
        }

        #endregion

        /// <summary>
        /// Gets the chunk which contains the given global tile position
        /// </summary>
        /// <param name="globalTilePosition">The global position of a tile which is in the desired chunk</param>
        /// <returns>null if there is no such chunk</returns>
        public Chunk<TValue> GetChunk(Point globalTilePosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update a single tile in a specific chunk
        /// </summary>
        /// <param name="globalTilePosition">The global coordinates of the tile</param>
        /// <param name="value">The value to give the specified tile</param>
        public void UpdateTile(Point globalTilePosition, TValue value)
        {
            var chunk = GetChunk(globalTilePosition);
            if (chunk == null)
                return;
            var localTilePosition = chunk.GetLocalPosition(globalTilePosition);
            chunk.UpdateTile(localTilePosition, value);
        }

        /// <summary>
        /// Loads a chunk 
        /// </summary>
        /// <param name="globalChunkPosition"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        public void CacheChunk(Point globalChunkPosition, TValue[] data, int startIndex)
        {
            throw new NotImplementedException();
        }
    }
}
