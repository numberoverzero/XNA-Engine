using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Tiles
{
    /// <summary>
    /// A section of items that can be loaded from or saved to disk
    /// (or another resource, such as a network connection)
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Chunk<TValue>
    {
        /// <summary>
        /// The values of the tiles in the chunk
        /// </summary>
        public TValue[] Tiles { get; protected set; }

        /// <summary>
        /// Set or Get a tile's value at the given local coordinates.
        /// 
        /// </summary>
        /// <param name="x">Local Chunk x-coordinate</param>
        /// <param name="y">Local Chunk y-coordinate</param>
        /// <returns></returns>
        public TValue this[int x, int y]
        {
            get
            {
                RangeCheckTile(x, y);
                return Tiles[x + y * Dimensions.X];
            }
            set
            {
                RangeCheckTile(x, y);
                Tiles[x + y * Dimensions.X] = value;
            }
        }

        int globalX, globalY;

        /// <summary>
        /// The global position of the upper-left-most tile in the chunk
        /// </summary>
        public Point GlobalPosition
        {
            get
            {
                return new Point(globalX, globalY);
            }
        }

        /// <summary>
        /// Width and Height of the chunk
        /// </summary>
        public Point Dimensions { get; protected set; }

        /// <summary>
        /// Whether or not the values loaded in the chunk are valid.
        /// False when the chunk has been cleared, or is partially loaded.
        /// </summary>
        public bool IsLoaded { get; protected set; }

        /// <summary>
        /// Creates a chunk whose top left corner is at x, y and with dimensions width x height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Chunk(int x, int y, int width, int height){
            globalX = x;
            globalY = y;
            Dimensions = new Point(width, height);
            InitializeTiles();
            IsLoaded = false;
        }

        /// <summary>
        /// Create a chunk whose top left corner is at position, with the given dimensions
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dimensions"></param>
        public Chunk(Point position, Point dimensions) 
            :this(position.X, position.Y, dimensions.X, dimensions.Y) { }

        private void InitializeTiles()
        {
            int n = Dimensions.X * Dimensions.Y;
            Tiles = new TValue[n];
            for (int i = 0; i < n; i++)
                Tiles[i] = default(TValue);
        }

        /// <summary>
        /// Update the value of a single tile.
        /// Location is local to the chunk coordinates
        /// </summary>
        /// <param name="localTilePosition">Position of the tile in chunk's coordinates</param>
        /// <param name="value">New value for the tile to take</param>
        public void UpdateTile(Point localTilePosition, TValue value)
        {
            Tiles[localTilePosition.X + Dimensions.X * localTilePosition.Y] = value;
        }

        /// <summary>
        /// Loads data from an array into the chunk.
        /// Note that while this won't necessarily fill all data in the chunk,
        /// it will flag the chunk as loaded.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        public void LoadData(TValue[] data, int startIndex)
        {
            Array.Copy(data, startIndex, Tiles, 0, Dimensions.X * Dimensions.Y);
            IsLoaded = true;
        }

        /// <summary>
        /// Clears the chunk's data.
        /// Note that this does not actually null the values,
        /// so the chunk can be used again by a tile manager.
        /// Do not read values from a chunk after it has been cleared
        /// until all of the values have been read over using LoadData
        /// </summary>
        public void ClearData()
        {
            IsLoaded = false;
        }

        /// <summary>
        /// Set the chunk's global position.  This is the position of the upper-left tile in the chunk.
        /// </summary>
        /// <param name="position"></param>
        public void SetGlobalPosition(Point position)
        {
            globalX = position.X;
            globalY = position.Y;
        }

        /// <summary>
        /// Calculates the local position of a tile in the chunk given its global location
        /// </summary>
        /// <param name="globalTilePosition"></param>
        /// <returns>The local coordinates of the tile in the chunk, or null if the tile is not in the chunk</returns>
        public Point GetLocalPosition(Point globalTilePosition)
        {
            Point localTilePosition = new Point(
                            globalTilePosition.X - globalX,
                            globalTilePosition.Y - globalY);
            if (localTilePosition.X < 0 || localTilePosition.X >= Dimensions.X
                || localTilePosition.Y < 0 || localTilePosition.Y >= Dimensions.Y)
                localTilePosition = new Point(-1, -1);
            return localTilePosition;
        }

        /// <summary>
        /// Calculates the global position of a tile in the world given its local chunk location
        /// </summary>
        /// <param name="localTilePosition"></param>
        /// <returns>The global coordinates of the tile in the world</returns>
        public Point GetGlobalPosition(Point localTilePosition)
        {
            return new Point(localTilePosition.X + globalX, localTilePosition.Y + globalY);
        }


        private void RangeCheckTile(int x, int y)
        {
            if (x < 0 || x >= Dimensions.X
                    || y < 0 || y >= Dimensions.Y)
                throw new IndexOutOfRangeException("x and y must be between 0 and Dimensions - 1");
        }
    }
}
