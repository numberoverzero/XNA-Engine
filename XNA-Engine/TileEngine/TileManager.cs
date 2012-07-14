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
        /// The absolute origin of the first chunk loaded.
        /// All chunks are referenced in relative terms to this chunk.
        /// </summary>
        /// <example>
        /// A player logs in at (550, 450) and has a 10x10-tile screen (These are the buffer dimensions).
        /// The origin chunk is the region 500-599x, 400-499y.
        /// 
        /// To read the chunk to the left (coordinates 400-499x, 400-499y)
        ///     We use GetChunkStartIndex(new Point(-1, 0)) 
        ///     Since we are ofsetting one chunk left (-1) and no offset in the y (0)
        /// </example>
        public Point GlobalOrigin { get; protected set; }

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

        public TileManager(Point bufferDimensions, Point chunkDimensions)
        {
            BufferDimensions = bufferDimensions; 
            ChunkDimensions = chunkDimensions;
            InitializeChunks();
            GlobalOrigin = Point.Zero;
        }

        void InitializeChunks()
        {
            int nchunks = BufferDimensions.X * BufferDimensions.Y;
            Chunks = new Chunk<TValue>[nchunks];
            for (int i = 0; i < nchunks; i++)
                Chunks[i] = new Chunk<TValue>(Point.Zero, ChunkDimensions);
        }

        public void SetGlobalOrigin(Point globalOrigin)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Re-center the tile manager at the specified chunk offset from the previous origin chunk.
        /// </summary>
        /// <example>
        /// If we are currently centered on (5,-1)
        /// and the character moves north 1 tile, we would want
        /// to re-center on (5,0).
        /// To do this, we call CenterAt(new point(0,1)).
        /// </example>
        /// <param name="relativeChunkPosition"></param>
        public void CenterAt(Point relativeChunkPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the chunk at the relative chunk position
        /// </summary>
        /// <param name="relativeChunkPosition">The chunk offset relative the origin chunk</param>
        /// <returns>null if there is no such chunk</returns>
        public Chunk<TValue> GetChunk(Point relativeChunkPosition)
        {
            var globalChunkPosition = GetGlobalChunkPosition(relativeChunkPosition);
            Chunk<TValue> chunk = null;
            foreach (var bufferedChunk in Chunks)
                if (bufferedChunk.GlobalPosition == globalChunkPosition)
                    return bufferedChunk;
            return chunk;
        }

        /// <summary>
        /// Gets the chunk at the absolute tile position
        /// </summary>
        /// <param name="absoluteChunkPosX">The x-coord of the tile in the upper-left corner of the chunk, in abs global coordinates</param>
        /// <param name="absoluteChunkPosY">The y-coord of the tile in the upper-left corner of the chunk, in abs global coordinates</param>
        /// <returns>null if there is no such chunk</returns>
        public Chunk<TValue> GetChunk(int absoluteChunkPosX, int absoluteChunkPosY)
        {
            var globalChunkPosition = new Point(absoluteChunkPosX, absoluteChunkPosY);
            Chunk<TValue> chunk = null;
            foreach (var bufferedChunk in Chunks)
                if (bufferedChunk.GlobalPosition == globalChunkPosition)
                    return bufferedChunk;
            return chunk;
        }

        /// <summary>
        /// Update a single tile in a specific chunk
        /// </summary>
        /// <param name="relativeChunkPosition">The chunk offset relative the origin chunk</param>
        /// <param name="localTilePosition">The x and y coordinates of the tile in the chunk</param>
        /// <param name="value">The value to give the specified tile</param>
        public void UpdateTile(Point relativeChunkPosition, Point localTilePosition, TValue value)
        {
            var chunk = GetChunk(relativeChunkPosition);
            if (chunk != null)
                chunk.UpdateTile(localTilePosition, value);
        }

        /// <summary>
        /// Loads a chunk 
        /// </summary>
        /// <param name="relativeChunkPosition"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        public void CacheChunk(Point relativeChunkPosition, TValue[] data, int startIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the relative chunk position of the chunk containing the given tile position.
        /// </summary>
        /// <param name="absoluteTilePos"></param>
        /// <returns></returns>
        public Point GetRelativeChunkPosition(Point absoluteTilePos)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the local coordinates (in its chunk) of a given tile position
        /// </summary>
        /// <param name="absoluteTilePos"></param>
        /// <returns></returns>
        public Point GetLocalTilePosition(Point absoluteTilePos)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Absolute position of the tile in the world
        /// </summary>
        /// <param name="relativeChunkPosition"></param>
        /// <param name="localTilePosition"></param>
        /// <returns></returns>
        public Point GetGlobalTilePosition(Point relativeChunkPosition, Point localTilePosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Absolute position of a chunk in the world as given by its relative offset w.r.t. the current GlobalOrigin
        /// </summary>
        /// <param name="relativeChunkPosition"></param>
        /// <returns></returns>
        public Point GetGlobalChunkPosition(Point relativeChunkPosition)
        {
            throw new NotImplementedException();
        }
    }
}
